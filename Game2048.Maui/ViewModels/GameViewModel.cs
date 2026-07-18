using CommunityToolkit.Mvvm.Input;
using Game2048.Core;
using Game2048.Core.DTOs;
using Game2048.Core.Enums;
using Game2048.Maui.Enums;
using Game2048.Core.Factories;
using Game2048.Core.Models;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Services;
using System.Collections.ObjectModel;

namespace Game2048.Maui.ViewModels;

public partial class GameViewModel : BindableObject, IDisposable
{
    private readonly Game _gameCore;

    private readonly GameConfig _gameConfig;
    private readonly IProfileManager _profileManager;
    private readonly IStatisticsManager _statisticsManager;
    private readonly IAchievementManager _achievementManager;
    public ObservableCollection<TileViewModel> Tiles { get; } = new();

    public event Func<IEnumerable<TileTransition>, Task>? TilesMoved;
    public event Func<IEnumerable<TileTransition>, Task>? TilesRemoved;
    public event Func<IEnumerable<TileTransition>, Task>? TilesCreated;

    private ActionInputQueue _actionQueue;
    public IAsyncRelayCommand MoveCommand { get; private set; }
    public IAsyncRelayCommand UndoCommand { get; private set; }
    public IRelayCommand RestartCommand { get; private set; }
    public IRelayCommand UndoLastAndContinue { get; private set; }
    public IRelayCommand ExtendCommand { get; private set; }
    public IRelayCommand OpenSettingsCommand { get; private set; }
    public IRelayCommand CloseSettingsCommand { get; private set; }

    public event Action? OnVictory;
    public event Action? OnGameOver;
    public event Action? OnRestart;
    public event Action? OnSettings;

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
        BestScore = _profileManager.GetBestScore(_gameConfig.GameMode) ?? 0;
    }

    private void UpdateScores()
    {
        if (Score != _gameCore.Grid.Score)
        {
            Score = _gameCore.Grid.Score;
        }

        if (Score > BestScore)
        {
            BestScore = Score;
            _profileManager.SaveBestScore(_gameConfig.GameMode, BestScore);
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

    private bool _isExtendedAllowed;

    public bool IsExtendedAllowed
    {
        get => _isExtendedAllowed;
        set { _isExtendedAllowed = value; OnPropertyChanged(); }
    }

    // start (for overlays and additional windows inside the gameview) 
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

    private bool _isBoardBlocked = false;
    public bool IsBoardBlocked
    {
        get => _isBoardBlocked;
        set
        {
            _isBoardBlocked = value;
            OnPropertyChanged();
        }
    }

    private bool _isGameOver;
    public bool IsGameOver
    {
        get => _isGameOver;
        set { _isGameOver = value; OnPropertyChanged(); }
    }

    private bool _isVictory;
    public bool IsVictory
    {
        get => _isVictory;
        set { _isVictory = value; OnPropertyChanged(); }
    }

    private bool _isSettings;
    public bool IsSettings
    {
        get => _isSettings;
        set { _isSettings = value; OnPropertyChanged(); }
    }
    //methods
    private void SetGameOverState() 
    {
        IsGameOver = true;
        IsBoardBlocked = true;

        IsVictory = false;
        IsActiveGame = false;
        IsSettings = false;
    }

    private void SetVictoryState()
    {
        IsVictory = true;
        IsBoardBlocked = true;

        IsGameOver = false;
        IsActiveGame = false;
        IsSettings = false;
    }

    private void SetSettingsState()
    {
        IsSettings = true;
        IsBoardBlocked = true;

        IsActiveGame = false;
    }

    private void SetActiveState()
    {
        IsActiveGame = true;

        IsSettings = false;
        IsBoardBlocked = false;
    }
    // end 

    public int Rows => _gameCore.Grid.Height;
    public int Columns => _gameCore.Grid.Width;

    public GameViewModel(GameConfig config, IProfileManager profileManager, IStatisticsManager statisticsManager, IAchievementManager achievementManager)
    {
        _gameCore = GameFactory.CreateGame(config);

        _profileManager = profileManager;
        _gameConfig = config;
        _statisticsManager = statisticsManager;
        _achievementManager = achievementManager;

        _actionQueue = new ActionInputQueue();
        MoveCommand = new AsyncRelayCommand<string>(OnMoveRequested);
        UndoCommand = new AsyncRelayCommand(OnUndoRequested);
        RestartCommand = new RelayCommand(OnRestartRequested);
        UndoLastAndContinue = new RelayCommand(OnUndoAndContinueRequested);
        ExtendCommand = new RelayCommand(OnExtendRequested);
        OpenSettingsCommand = new RelayCommand(OnOpenSettingsRequested);
        CloseSettingsCommand = new RelayCommand(OnCloseSettingsRequested);
        
        _isUndoEnabled = config.GameMode != GameModeType.Classic;

        _isExtendedAllowed = config.GameMode != GameModeType.Compact && 
                             config.GameMode != GameModeType.Extended &&
                             config.GameMode != GameModeType.ChillZone;

        _gameCore.OnVictory += HandleOnVictory;
        _gameCore.OnGameOver += HandleOnGameOver;
        _gameCore.OnStateChanged += HandleOnStateChanged;

        _achievementManager.OnAchievementUnlocked += HandleAchievementUnlocked;
    }

    private void HandleAchievementUnlocked(AchievementType achievementType)
    {
        _profileManager.UnlockAchievement(achievementType);
        _actionQueue.Enqueue(async () =>
        {
            await _profileManager.SaveCurrentProfileAsync();
        });
    }

    public void Dispose()
    {
        _gameCore.OnVictory -= HandleOnVictory;
        _gameCore.OnGameOver -= HandleOnGameOver;
        _gameCore.OnStateChanged -= HandleOnStateChanged;
        _achievementManager.OnAchievementUnlocked -= HandleAchievementUnlocked;
    }

    public bool TryLoadSave()
    {
        if(_profileManager.CurrentProfile is null) return false;

        var gameMode = _gameConfig.GameMode;
        var save = _profileManager.LoadCurrentGame(gameMode);
        if (save is not null)
        {
            _gameCore.ColdLoadFromSave(save.LastState, save.History);
            LoadBestScore();
            return true;
        }
        return false;
    }

    public void StartGame()
    {
        if (!TryLoadSave())
        {
            _gameCore.Clear();
            Tiles.Clear();
            _gameCore.SpawnMultipleTiles(2);
        }

        SyncState();
    }

    public void StartNewGame()
    {
        _gameCore.Clear();
        Tiles.Clear();
        _gameCore.SpawnMultipleTiles(2);
        SyncState();
        SaveCurrentGameState();
    }

    private void SyncState()
    {
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

        _statisticsManager.Moved();
        _statisticsManager.Merged(transitions.Where(x => x.Type == TileTransitionType.Merge).Count());

        _achievementManager.CheckAllAchievements(_gameCore.GetCurrentGridState(), transitions, _statisticsManager);
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

        _statisticsManager.Undone();
        _statisticsManager.Respawned(transitions.Where(x => x.Type == TileTransitionType.Respawn).Count());

        _achievementManager.CheckAllAchievements(_gameCore.GetCurrentGridState(), transitions, _statisticsManager);
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
        SetVictoryState();
        OnVictory?.Invoke();

        _statisticsManager.GameEnded(hasWon : true);
        _achievementManager.CheckSpecialAchievements(_statisticsManager);
        _statisticsManager.Push();
        _achievementManager.CheckGlobalAchievements();

        _actionQueue.Enqueue(async () =>
        {
            await _profileManager.SaveCurrentProfileAsync();
        });
    }

    private void HandleOnGameOver()
    {
        SetGameOverState();
        OnGameOver?.Invoke();

        _statisticsManager.GameEnded(hasWon: false);
        _achievementManager.CheckSpecialAchievements(_statisticsManager);
        _statisticsManager.Push();
        _achievementManager.CheckGlobalAchievements();

        _actionQueue.Enqueue(async () =>
        {
            await _profileManager.SaveCurrentProfileAsync();
        });
    }

    private void SaveCurrentGameState()
    {
        var gameMode = _gameConfig.GameMode;

        _profileManager.SaveCurrentGame(
            gameMode,
            _gameCore.GetCurrentGridState(),
            _gameCore.GetHistoryState());
    }

    private void HandleOnStateChanged()
    {
        if (_profileManager.CurrentProfile is not null)
        {
            SaveCurrentGameState();
        }
    }

    private void OnRestartRequested()
    {
        _actionQueue.Clear();

        _statisticsManager.Push();

        _achievementManager.CheckGlobalAchievements();

        _actionQueue.Enqueue(async () =>
        {
            await _profileManager.SaveCurrentProfileAsync();

            StartNewGame();
            OnRestart?.Invoke();
            SetActiveState();
        });
    }

    public async Task OnBackToMenu()
    {
        _actionQueue.Clear();

        _statisticsManager.Push();

        await _profileManager.SaveCurrentProfileAsync();
    }

    public async Task OnGoToMenu()
    {
        _actionQueue.Clear();

        _statisticsManager.Push();

        StartNewGame();
        SetActiveState();

        HandleOnStateChanged();

        await _profileManager.SaveCurrentProfileAsync();
    }

    private void OnUndoAndContinueRequested()
    {
        _actionQueue.Clear();

        _statisticsManager.Push();

        _achievementManager.CheckGlobalAchievements();

        _actionQueue.Enqueue(async () =>
        {
            await _profileManager.SaveCurrentProfileAsync();

            await Task.Run(() => { _gameCore.UndoMultiple(5); });

            Score = _gameCore.Grid.Score;
            HistoryCount = _gameCore.HistoryCount;

            Tiles.Clear();
            SyncTiles();

            OnRestart?.Invoke();

            SetActiveState();
        });
    }

    private void OnExtendRequested()
    {
        if(!IsExtendedAllowed) return;

        _actionQueue.Clear();

        _statisticsManager.Push();

        _achievementManager.CheckGlobalAchievements();

        _actionQueue.Enqueue(async () =>
        {
            await _profileManager.SaveCurrentProfileAsync();

            IsExtendedAllowed = false;

            _gameCore.Extend(4096);

            SetActiveState();
        });
    }

    private void OnOpenSettingsRequested()
    {
        SetSettingsState();
        OnSettings?.Invoke();
    }

    private void OnCloseSettingsRequested()
    {
        SetActiveState();
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
