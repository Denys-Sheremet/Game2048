using Game2048.Core;
using Game2048.Core.DTOs;
using Game2048.Core.Mechanics;
using CommunityToolkit.Mvvm.Input;
using Game2048.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Game2048.Maui.ViewModels;

public class GameViewModel : BindableObject
{
    private readonly Game _gameCore;
    public ObservableCollection<TileViewModel> Tiles { get; } = new();

    public event Func<IEnumerable<TileTransition>, Task>? TilesMoved;
    public event Func<IEnumerable<TileTransition>, Task>? TilesRemoved;
    public event Func<IEnumerable<TileTransition>, Task>? TilesCreated;

    public IAsyncRelayCommand MoveCommand { get; private set; }
    public IAsyncRelayCommand UndoCommand { get; private set; }

    private bool _isAnimating;

    private int _score;
    public int Score
    {
        get => _score;
        set { _score = value; OnPropertyChanged(); }
    }

    public int Rows => _gameCore.Grid.Height;
    public int Columns => _gameCore.Grid.Width;

    public GameViewModel(int rows, int cols)
    {
        _gameCore = GameFactory.CreateStandardGame(cols, rows);
        MoveCommand = new AsyncRelayCommand<string>(ExecuteMoveAsync);
        UndoCommand = new AsyncRelayCommand(ExecuteUndoAsync);
    }

    public void StartNewGame()
    {
        _gameCore.SpawnMultipleTiles(2);
        SyncTiles();
        Score = _gameCore.Grid.Score;
    }

    private async Task ExecuteMoveAsync(string? directionStr)
    {
        if (_isAnimating || directionStr is null) return;
        if (!Enum.TryParse<MoveDirection>(directionStr, out MoveDirection dir)) return;

        var transitions = _gameCore.Move(dir);
        SyncTiles();

        _isAnimating = true;

        try
        {
            await InvokeTransition(TilesMoved, transitions.Where(x => x.Type == TileTransitionType.Move ||
                                                                      x.Type == TileTransitionType.Merge));

            await InvokeTransition(TilesRemoved, transitions.Where(x => x.Type == TileTransitionType.Disappear ||
                                                                        x.Type == TileTransitionType.Merge || 
                                                                        x.Type == TileTransitionType.Split));

            await InvokeTransition(TilesCreated, transitions.Where(x => x.Type == TileTransitionType.Spawn ||
                                                                        x.Type == TileTransitionType.Result ||
                                                                        x.Type == TileTransitionType.Respawn));
            Score = _gameCore.Grid.Score;
        }
        finally
        {
            _isAnimating = false;
        }
    }

    private async Task ExecuteUndoAsync()
    {
        if (_isAnimating) return;

        var transitions = _gameCore.Undo();

        // --- ДЕБАГ АНАЛИЗАТОРА ---
        System.Diagnostics.Debug.WriteLine("=== UNDO ANALYSIS START ===");
        foreach (var tr in transitions)
        {
            System.Diagnostics.Debug.WriteLine(
                $"ID: {tr.TileId} | Type: {tr.Type} | " +
                $"From: ({tr.FromX}, {tr.FromY}) -> To: ({tr.ToX}, {tr.ToY})");
        }
        System.Diagnostics.Debug.WriteLine("=== UNDO ANALYSIS END ===");

        if (!transitions.Any()) return;

        SyncTiles();

        _isAnimating = true;

        try
        {
            await InvokeTransition(TilesMoved, transitions.Where(x => x.Type == TileTransitionType.Move));
            await InvokeTransition(TilesRemoved, transitions.Where(x => x.Type == TileTransitionType.Disappear || 
                                                                        x.Type == TileTransitionType.Split));
            await InvokeTransition(TilesCreated, transitions.Where(x => x.Type == TileTransitionType.Respawn));

            Score = _gameCore.Grid.Score;
        }
        finally
        {
            _isAnimating = false;
        }
    }

    public void SyncTiles()
    {
        //deletion of absent tiles
        var stateSnapshot = _gameCore.GetCurrentGridState();
        var activeIds = stateSnapshot.TileSnapshots.Select(s => s.Id).ToHashSet();

        var toRemove = Tiles.Where(t => !activeIds.Contains(t.Id)).ToList();
        foreach (var vm in toRemove)
        {
            Tiles.Remove(vm);
        }

        //creation or update of present tiles
        foreach (var ss in stateSnapshot.TileSnapshots)
        {
            var existing = Tiles.FirstOrDefault(t => t.Id == ss.Id);

            if (existing is null)
            {
                    Tiles.Add(new TileViewModel(_gameCore.TileRegistry[ss.Id]!, ss.PosY, ss.PosX));
            }
            else
            {
                existing.SyncData();
            }
        }
    }

    

    private async Task InvokeTransition(Func<IEnumerable<TileTransition>, Task>? phaseEvent,IEnumerable<TileTransition> transitions)
    {
        if (phaseEvent is null || !transitions.Any()) return;

        var tasks = phaseEvent.GetInvocationList()
                              .Cast<Func<IEnumerable<TileTransition>, Task>>()
                              .Select(func => func(transitions));

        await Task.WhenAll(tasks);
    }
}
