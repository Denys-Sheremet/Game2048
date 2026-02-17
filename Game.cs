using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048
{
    internal class Game
    {
        public Grid Grid { get; private set; }
        private Stack<StateSnapshot> _history;
        //Safe system of registry with readonly interface for outter calls (pointer leak issue)
        private TileRegistry _tileRegistry;
        public IReadOnlyTileRegistry TileRegistry => _tileRegistry; //getter
        private int _nextTileId;
        private Random _random;
        public bool IsGameOver { get; private set; } = false; //default state will be false from the start (no need to initialize)


        public Game(Grid grid)
        {
            Grid = grid;
            _history = new Stack<StateSnapshot>();
            _tileRegistry = new TileRegistry();
            _nextTileId = 1;
            _random = new Random();

            SpawnNewTile();
            SpawnNewTile();
        }

        public void SpawnNewTile()
        {
            List<(int, int)> emptyCells = Grid.GetEmptyCells();
            if (emptyCells.Count < 1)
            {
                CheckForGameOver();//todo
                return;
            } 
            else
            {
                (int x, int y) chosen = emptyCells[_random.Next(emptyCells.Count)];
                int value = _random.Next(10) == 0 ? 4 : 2; //10% that 4 will appear
                Tile newTile = new Tile(GetNextTileId(), chosen.x, chosen.y, chosen.x, chosen.y, false, value);
                Grid[chosen.x, chosen.y] = newTile;
                _tileRegistry.Register(newTile);
            }

        }

        public void Move(MoveDirection direction)
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
                SpawnNewTile();
            }
            else
            {
                CheckForGameOver();
            }
        }

        public void Undo()
        {
            if (_history.Count == 0) return;

            StateSnapshot stateSnapshot = _history.Pop();
            Grid.Restore(stateSnapshot);

            _tileRegistry.Clear();
            for (int i = 0; i < Grid.GetCount(); i++) 
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

        public void OnGameOver() 
        { 
            IsGameOver = true;
        }

        public int GetNextTileId() => _nextTileId++;

    }
}
