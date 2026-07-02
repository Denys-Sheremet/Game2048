using CommunityToolkit.Mvvm.Input;
using Game2048.Core;
using Game2048.Core.DTOs;
using Game2048.Core.Enums;
using Game2048.Core.Factories;
using Game2048.Core.Interfaces;
using Game2048.Core.Models;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Models;
using Game2048.Maui.Services;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Game2048.Maui.ViewModels;

public partial class GameViewModel : BindableObject
{
    private readonly Game _gameCore;

    private readonly GameConfig _gameConfig;
    private readonly ISaveService _saveService;
    private readonly IProfileManager _profileManager;
    public ObservableCollection<TileViewModel> Tiles { get; } = new();

    public event Func<IEnumerable<TileTransition>, Task>? TilesMoved;
    public event Func<IEnumerable<TileTransition>, Task>? TilesRemoved;
    public event Func<IEnumerable<TileTransition>, Task>? TilesCreated;

    private ActionInputQueue _actionQueue;
    public IAsyncRelayCommand MoveCommand { get; private set; }
    public IAsyncRelayCommand UndoCommand { get; private set; }
    public IAsyncRelayCommand RestartCommand { get; private set; }
    public IRelayCommand UndoLastAndContinue { get; private set; }

    public event Action? OnVictory;
    public event Action? OnGameOver;
    public event Action? OnRestart;

    private bool _isActiveGame = true;
    public bool IsActiveGame
    {
        get => _isActiveGame;
        set
        {
            _isActiveGame = value;
            OnPropertyChanged();
        }
    }

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

    public GameViewModel(GameConfig config, ISaveService saveService, IProfileManager profileManager)
    {
        _gameCore = GameFactory.CreateGame(config);
        _saveService = saveService;
        _profileManager = profileManager;
        _gameConfig = config;

        LoadGameSave(config);

        _actionQueue = new ActionInputQueue();
        MoveCommand = new AsyncRelayCommand<string>(OnMoveRequested);
        UndoCommand = new AsyncRelayCommand(OnUndoRequested);
        RestartCommand = new AsyncRelayCommand(OnRestartRequested);
        UndoLastAndContinue = new RelayCommand(OnUndoAndContinueRequested);
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
        _gameCore.OnStateChanged += HandleOnStateChanged;
    }

    public void LoadGameSave(GameConfig config)
    {
        ArgumentNullException.ThrowIfNull(_profileManager.CurrentProfile);

        if (_profileManager.CurrentProfile.Saves
            .TryGetValue(config.GameMode, out GameSessionSave? save))
        {
            _gameCore.Clear();
            _gameCore.Grid.ColdRestore(save.LastState);
            if (save.History is null) return;
            _gameCore.HistoryColdRestore(save.History);
        }
    }

    public void StartNewGame()
    {
        _gameCore.Clear();
        Tiles.Clear();
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
        IsActiveGame = false;
        OnVictory?.Invoke();
    }

    private void HandleOnGameOver()
    {
        IsEndGame = true;
        IsActiveGame = false;
        OnGameOver?.Invoke();
    }

    private void HandleOnStateChanged()
    {
        if (_profileManager.CurrentProfile is not null)
        {
            var gameMode = _gameConfig.GameMode;
            var gameState = _gameCore.GetCurrentGridState();
            var history = _gameCore.GetHistoryState();
            _saveService.SaveCurrentGame(gameMode, gameState, history);
        }
    }

    private async Task OnRestartRequested()
    {
        _actionQueue.Clear();

        _actionQueue.Enqueue(() =>
        {
            StartNewGame();
            OnRestart?.Invoke();
            IsEndGame = false;
            IsActiveGame = true;
            return Task.CompletedTask;
        });
        
        await Task.CompletedTask;
    }
    private void OnUndoAndContinueRequested()
    {
        _actionQueue.Clear();
        _actionQueue.Enqueue(async () =>
        {
            await Task.Run(() => { _gameCore.UndoMultiple(5); });

            Score = _gameCore.Grid.Score;
            HistoryCount = _gameCore.HistoryCount;

            Tiles.Clear();
            SyncTiles();

            OnRestart?.Invoke();

            IsEndGame = false;
            IsActiveGame = true;
        });
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
