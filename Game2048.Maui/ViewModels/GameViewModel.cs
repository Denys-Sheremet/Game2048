using Game2048.Core;
using Game2048.Core.DTOs;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Game2048.Maui.Services;
using Game2048.Core.Factories;
using Game2048.Core.Enums;
using Game2048.Core.Models;
using System.Runtime.CompilerServices;

namespace Game2048.Maui.ViewModels;

public partial class GameViewModel : BindableObject
{
    private readonly Game _gameCore;
    public ObservableCollection<TileViewModel> Tiles { get; } = new();

    public event Func<IEnumerable<TileTransition>, Task>? TilesMoved;
    public event Func<IEnumerable<TileTransition>, Task>? TilesRemoved;
    public event Func<IEnumerable<TileTransition>, Task>? TilesCreated;

    private ActionInputQueue _actionQueue;
    public IAsyncRelayCommand MoveCommand { get; private set; }
    public IAsyncRelayCommand UndoCommand { get; private set; }
    public IAsyncRelayCommand RestartCommand { get; private set; }
    public IAsyncRelayCommand UndoLastAndContinue { get; private set; }

    public event Action? OnVictory;
    public event Action? OnGameOver;
    public event Action? OnRestart;

    private bool _isEndGame = false;
    public bool IsEndGame
    {
        get => _isEndGame;
        set
        {
            _isEndGame = value;
            OnPropertyChanged();
        }
    }

    private int _score;
    public int Score
    {
        get => _score;
        set { _score = value; OnPropertyChanged(); }
    }

    //TODO
    private int _bestScore;
    public int BestScore
    {
        get => _bestScore;
        set { _bestScore = value; OnPropertyChanged(); }
    }

    public void LoadBestScore()
    {
        BestScore = Preferences.Default.Get("best_score", 0);
    }

    private void UpdateScores()
    {
        Score = _gameCore.Grid.Score;

        if (Score > BestScore)
        {
            BestScore = Score;
            Preferences.Default.Set("best_score", BestScore);
        }
    }

    //TODO

    private int _historyCount;
    public int HistoryCount
    {
        get => _historyCount;
        set { _historyCount = value; OnPropertyChanged(); } 
    }

    private bool _isUndoEnabled;

    public bool IsUndoEnabled
    {
        get => _isUndoEnabled;
        set { _isUndoEnabled = value; OnPropertyChanged(); }
    }

    public int Rows => _gameCore.Grid.Height;
    public int Columns => _gameCore.Grid.Width;

    public GameViewModel(GameConfig config)
    {
        _gameCore = GameFactory.CreateGame(config);
        _actionQueue = new ActionInputQueue();
        MoveCommand = new AsyncRelayCommand<string>(OnMoveRequested);
        UndoCommand = new AsyncRelayCommand(OnUndoRequested);
        RestartCommand = new AsyncRelayCommand(OnRestartRequested);
        UndoLastAndContinue = new AsyncRelayCommand(OnUndoAndContinueRequested);
        if (config.GameMode == GameModeType.Classic)
        {
            _isUndoEnabled = false;
        }
        else 
        { 
            _isUndoEnabled = true; 
        }

        _gameCore.OnVictory += HandleOnVictory;
        _gameCore.OnGameOver += HandleOnGameOver;
    }

    public void StartNewGame()
    {
        _gameCore.Clear();
        _gameCore.SpawnMultipleTiles(2);
        SyncTiles();
        HistoryCount = _gameCore.HistoryCount;
        Score = _gameCore.Grid.Score;
        LoadBestScore();
    }

    private async Task OnMoveRequested(string? directionStr)
    {
        if (directionStr is null) return;
        if (!Enum.TryParse<MoveDirection>(directionStr, out MoveDirection direction)) return;

        _actionQueue.Enqueue(() => ExecuteMoveAsync(direction));

        await Task.CompletedTask;
    }

    private async Task ExecuteMoveAsync(MoveDirection direction)
    {
        var transitions = _gameCore.Move(direction);

        SyncTiles();

        HistoryCount = _gameCore.HistoryCount;

        await InvokeTransition(TilesMoved, transitions.Where(x => x.Type == TileTransitionType.Move ||
                                                                    x.Type == TileTransitionType.Merge));

        await InvokeTransition(TilesRemoved, transitions.Where(x => x.Type == TileTransitionType.Disappear ||
                                                                    x.Type == TileTransitionType.Merge || 
                                                                    x.Type == TileTransitionType.Split));

        await InvokeTransition(TilesCreated, transitions.Where(x => x.Type == TileTransitionType.Spawn ||
                                                                    x.Type == TileTransitionType.Result ||
                                                                    x.Type == TileTransitionType.Respawn));
        UpdateScores();
    }

    private async Task OnUndoRequested()
    {
        _actionQueue.Enqueue(() => ExecuteUndoAsync());
        await Task.CompletedTask;
    }

    private async Task ExecuteUndoAsync(List<TileTransition>? givenTransitions = null)
    {
        List<TileTransition> transitions;

        if (givenTransitions is null)
        {
            transitions = _gameCore.Undo();
        } 
        else
        {
            transitions = givenTransitions;
        }


        if (!transitions.Any()) return;

        SyncTiles();

        HistoryCount = _gameCore.HistoryCount;

        await InvokeTransition(TilesMoved, transitions.Where(x => x.Type == TileTransitionType.Move));

        await InvokeTransition(TilesRemoved, transitions.Where(x => x.Type == TileTransitionType.Disappear || 
                                                                    x.Type == TileTransitionType.Split));
        await InvokeTransition(TilesCreated, transitions.Where(x => x.Type == TileTransitionType.Respawn));

        UpdateScores();
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

    private void HandleOnVictory()
    {
        IsEndGame = true;
        OnVictory?.Invoke();
    }

    private void HandleOnGameOver()
    {
        IsEndGame = true;
        OnGameOver?.Invoke();
    }

    private async Task OnRestartRequested()
    {
        _actionQueue.Enqueue(() =>
        {
            StartNewGame();
            OnRestart?.Invoke();
            IsEndGame = false;
            return Task.CompletedTask;
        });
        await Task.CompletedTask;
    }
    private async Task OnUndoAndContinueRequested()
    {
        var transitions = _gameCore.UndoMultiple(5);
        await ExecuteUndoAsync(transitions);
        OnRestart?.Invoke();
        IsEndGame = false;
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
