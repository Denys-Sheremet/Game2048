using Game2048.Core.DTOs;
using System.Collections.Generic;

namespace Game2048.Core;

public class Game
{
    public Grid Grid { get; private set; }
    public bool IsGameOver { get; private set; } = false;
    public bool IsVictory { get; private set; } = false;
    public int HistoryCount => _history.Count;

    private readonly ITileSpawner _spawner;
    private readonly IHistoryManager _history;
    private readonly ITileRegistry _tileRegistry;
    private readonly IRandomProvider _random;
    public IReadOnlyTileRegistry TileRegistry => _tileRegistry;
    private int _nextTileId = 1;
    private int _maxTileValue = 2048;

    //Events to invoke changes for UI
    //As we have one sender Action class is perfect instead of EventHandler
    public event Action? OnStateChanged; 
    public event Action<int>? OnScoreGained;
    public event Action? OnVictory;
    public event Action? OnGameOver;


    public Game(Grid grid, ITileSpawner spawner, IHistoryManager history, ITileRegistry tileRegistry, IRandomProvider random)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(history);
        ArgumentNullException.ThrowIfNull(tileRegistry);
        ArgumentNullException.ThrowIfNull(random);
        ArgumentNullException.ThrowIfNull(spawner);

        Grid = grid;
        _history = history;
        _tileRegistry = tileRegistry;
        _random = random;
        _spawner = spawner;
    }

    public void SpawnNewTile()
    {
        bool isSpawned = _spawner.Spawn(Grid, _tileRegistry, GetNextTileId(), _random);
        if (!isSpawned) 
        {
            CheckForGameOver();
        }
    }

    public void SpawnMultipleTiles(int count)
    {
        for (int i = 0; i < count; i++) 
        { 
            SpawnNewTile();
        }
    }

    public bool TrySpawnNewTileAt(int x, int y, int? newValue = null)
    {
        if (newValue is null)
        {
            return _spawner
                .TrySpawnAt
                (
                    grid: Grid, 
                    registry: _tileRegistry, 
                    nextId: GetNextTileId(), 
                    x: x, 
                    y: y, 
                    value: null, 
                    random: _random
                );
        }
        return _spawner
            .TrySpawnAt
            (
                grid: Grid,
                registry: _tileRegistry,
                nextId: GetNextTileId(),
                x: x,
                y: y,
                value: newValue,
                random: null
            );
    }

    public List<TileTransition> Move(MoveDirection direction, bool withSpawn = true)
    {
        StateSnapshot before = this.Grid.CreateSnapshot(GetNextTileId(false));

        Grid.SyncAllPrevious();

        StateSnapshot snapshot = before;
        bool moved = false;
        int score = 0;

        bool isHorizontal = (direction == MoveDirection.Left || direction == MoveDirection.Right);
        bool isReversed = (direction == MoveDirection.Right || direction == MoveDirection.Down);
        int length = isHorizontal ? Grid.Height : Grid.Width;

        for (int i = 0; i < length; i++)
        {
            Tile?[] line = isHorizontal ? Grid.GetRow(i) : Grid.GetColumn(i);
            if (isReversed) Array.Reverse(line); //in-place

            GameMechanics.ProcessResult result = GameMechanics.ProcessLine(line, () => GetNextTileId());

            if (result.WasMoved)
            {
                //registry
                _tileRegistry.UnregisterMany(result.MergedTiles);
                _tileRegistry.RegisterMany
                    (
                        result.NewLine.OfType<Tile>().Where(t => t.IsMerged)
                    );
                //registry

                moved = true;
                score += result.EarnedScore;

                if (isReversed) Array.Reverse(result.NewLine); //in-place

                if (isHorizontal)
                {
                    Grid.SetRow(i, result.NewLine);
                }
                else
                {
                    Grid.SetColumn(i, result.NewLine);
                }
            }
        }

        if(moved)
        {
            _history.Push(snapshot);
            Grid.Score += score;
            if (withSpawn)
                SpawnNewTile();

            if (score > 0)
            {
                OnScoreGained?.Invoke(score);
            }

            CheckForVictory();
            OnStateChanged?.Invoke();
        }
        else
        {
            CheckForGameOver();
            return new List<TileTransition>();
        }

        StateSnapshot after = this.Grid.CreateSnapshot(GetNextTileId(false));

        return TransitionAnalyzer.Analyze(before, after);
    }

    public List<TileTransition> Undo()
    {
        StateSnapshot before = this.Grid.CreateSnapshot(GetNextTileId(false));

        if (HistoryIsEmpty()) return new List<TileTransition>();

        StateSnapshot stateSnapshot = _history.Pop()!;
        Grid.Restore(stateSnapshot);
        _nextTileId = stateSnapshot.NextId;

        _tileRegistry.Clear();
        for (int i = 0; i < Grid.Count; i++) 
        {
            _tileRegistry.Register(Grid[i]);
        } 

        IsGameOver = false;

        StateSnapshot after = this.Grid.CreateSnapshot(GetNextTileId(false));

        return TransitionAnalyzer.Analyze(before, after);
    }

    public void CheckForGameOver()
    {
        if (Grid.GetEmptyCells().Any()) return;

        bool canMergeHorizontal = Enumerable.Range(0, Grid.Width - 1)
            .SelectMany
            (
                x => Enumerable.Range(0, Grid.Height)
                .Select(y => new {curr = Grid[x, y], next = Grid[x + 1, y] })
            )
            .Any(pair => pair.curr?.Value == pair.next?.Value);

        bool canMergeVertical = Enumerable.Range(0, Grid.Width)
            .SelectMany
            (
                x => Enumerable.Range(0, Grid.Height - 1)
                .Select(y => new { curr = Grid[x, y], next = Grid[x, y + 1] })
            )
            .Any(pair => pair.curr?.Value == pair.next?.Value);

        if (canMergeHorizontal || canMergeVertical) return;

        Over();
    }

    public void CheckForVictory()
    {
        if (!IsVictory && this.Grid.CheckForValue(_maxTileValue))
        {
            Victory();
        }
    }
    private void Over()
    {
        IsGameOver = true;
        OnGameOver?.Invoke();
    }

    private void Victory() 
    {
        IsVictory = true;
        OnVictory?.Invoke();
    } 

    public void SetMaxValue(int maxTileValue)
    {
        if (
            !(
                maxTileValue > 0
                &&
                (maxTileValue & (maxTileValue - 1)) == 0 //check if value is a power of 2
            )
        ) throw new ArgumentException("The value is invalid or not a power of 2");

        _maxTileValue = maxTileValue;
    }

    public bool HistoryIsEmpty() => _history.IsEmpty;

    public int GetNextTileId(bool withIncrement = true) 
    {
        if (withIncrement)
        {
            return _nextTileId++;
        }
        return _nextTileId;
    }

    public StateSnapshot GetCurrentGridState()
    {
        return this.Grid.CreateSnapshot(_nextTileId);
    }
}
