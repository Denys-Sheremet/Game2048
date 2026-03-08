using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048
{
    public class Game
    {
        public Grid Grid { get; private set; }
        public bool IsGameOver { get; private set; } = false;
        public int HistoryCount => _history.Count;

        private readonly ITileSpawner _spawner;
        private readonly IHistoryManager _history;
        private readonly ITileRegistry _tileRegistry;
        private readonly IRandomProvider _random;
        public IReadOnlyTileRegistry TileRegistry => _tileRegistry;
        private int _nextTileId = 1;

        //Events to invoke changes for UI in future updates
        //As we have one sender Action class is perfect instead of EventHandler
        public event Action? OnStateChanged; 
        public event Action<int>? OnScoreGained;


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

        public void Move(MoveDirection direction, bool withSpawn = true)
        {
            Grid.SyncAllPrevious();

            StateSnapshot snapshot = Grid.CreateSnapshot();
            bool moved = false;
            int score = 0;

            bool isHorisontal = (direction == MoveDirection.Left || direction == MoveDirection.Right);
            bool isReversed = (direction == MoveDirection.Right || direction == MoveDirection.Down);
            int length = isHorisontal ? Grid.Height : Grid.Width;

            for (int i = 0; i < length; i++)
            {
                Tile?[] line = isHorisontal ? Grid.GetRow(i) : Grid.GetColumn(i);
                if (isReversed) Array.Reverse(line); //in-place

                GameMechanics.ProcessResult result = GameMechanics.ProcessLine(line, GetNextTileId);

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

                    if (isHorisontal)
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

                OnStateChanged?.Invoke();
            }
            else
            {
                CheckForGameOver();
            }
        }

        public void Undo()
        {
            if (HistoryIsEmpty()) return;

            StateSnapshot stateSnapshot = _history.Pop()!;
            Grid.Restore(stateSnapshot);

            _tileRegistry.Clear();
            for (int i = 0; i < Grid.Count; i++) 
            {
                _tileRegistry.Register(Grid[i]);
            } 

            IsGameOver = false;
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

            OnGameOver();
        }

        private void OnGameOver() 
        { 
            IsGameOver = true;
        }

        public bool HistoryIsEmpty() => _history.IsEmpty;

        public int GetNextTileId() => _nextTileId++;

    }
}
