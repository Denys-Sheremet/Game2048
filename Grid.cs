using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048;

internal class Grid
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    private List<Tile> _tiles;

    private Tile?[,] _field;

    public int Score { get; set; }

    //Constructor for empty grid
    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        _field = new Tile?[width, height];
        _tiles = new List<Tile>();
        Score = 0;
    }

    //restore from snapshot
    public void Restore(StateSnapshot ss)
    {
        Dictionary<int, Tile> currentTiles = _tiles.ToDictionary(t => t.Id);
        Dictionary<int, Tile> restoredTiles = new Dictionary<int, Tile>();

        Array.Clear(_field, 0, _field.Length);
        _tiles.Clear();
        this.Score = ss.Score;

        for (int i = 0; i < ss.TileSnapshots.Count; i++)
        {
            TileSnapshot ts = ss.TileSnapshots[i];
            int id = ts.Id;

            int startX = currentTiles.ContainsKey(id) ? currentTiles[id].PosX : ts.PosX;
            int startY = currentTiles.ContainsKey(id) ? currentTiles[id].PosY : ts.PosY;
            Tile restored = new Tile(id, ts.PosX, ts.PosY, startX, startY, ts.Parents.HasValue, ts.Value);

            if (ts.Parents.HasValue) 
            { 
                restored.SetParents(ts.Parents.Value.Id1, ts.Parents.Value.Id2); 
            }
            restoredTiles.Add(id, restored);

            this[ts.PosX, ts.PosY] = restored;
        }

        foreach (Tile t in currentTiles.Values.Where(t => t.IsMerged))
        {
            if (t.Parents.HasValue)
            {
                restoredTiles[t.Parents.Value.Id1].SetPrevious(t.PosX, t.PosY);
                restoredTiles[t.Parents.Value.Id2].SetPrevious(t.PosX, t.PosY);
            }
        }
        
        
    }

    //To get to the grid fields easily
    public Tile? this[int x, int y]
    {
        get
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                throw new IndexOutOfRangeException($"coordinates {x} or {y} provided aren't correct");
            return _field[x, y];
        }
        set 
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                _field[x, y] = value;

                if(value  != null)
                {
                    if (!_tiles.Contains(value))
                        _tiles.Add(value);
                }
                
            }
        }
    }

    //To get to the list of tiles (cycles)
    public Tile this[int i]
    {
        get
        {
            if (i >= _tiles.Count || i < 0)
                throw new IndexOutOfRangeException("Incorrect index provided");
            return _tiles[i];

        }
    }

    public Tile?[] GetRow(int y)
    {
        Tile?[] row = new Tile?[Width];
        for (int x = 0; x < Width; x++)
        {
            row[x] = _field[x, y];
        }
        return row;
    }

    //console version
    public void SetRow(int y, Tile?[] row)
    {
        for (int x = 0; x < Width; x++) 
        {
            Tile? oldTile = _field[x, y];
            Tile? newTile = row[x];

            if (!row.Contains(oldTile)) 
            {
                _tiles.Remove(oldTile);
            }

            _field[x, y] = newTile;

            if (newTile != null)
            {
                newTile.UpdatePosition(x, y);

                if (!_tiles.Contains(newTile))
                    _tiles.Add(newTile);
            }
        }
    }

    public Tile?[] GetColumn(int x)
    {
        Tile?[] column = new Tile?[Height];
        for (int y = 0; y < Height; y++) 
        {
            column[y] = _field[x, y];
        }
        return column;
    }

    public void SetColumn(int x, Tile?[] column)
    {
        for (int y = 0; y < Height; y++) 
        {
            Tile? oldTile = _field[x, y];
            Tile? newTile = column[y];

            if (!column.Contains(oldTile))
            {
                _tiles.Remove(oldTile);
            }

            _field[x, y] = newTile;

            if (newTile != null)
            {
                newTile.UpdatePosition(x, y);

                if (!_tiles.Contains(newTile))
                    _tiles.Add(newTile);
            }
        }
    }

    //Count of list of tiles (cycles)
    public int GetCount() => _tiles.Count;

    public StateSnapshot CreateSnapshot() 
    {
        return new StateSnapshot(this.Width, this.Height, this.Score, this._tiles);
    }

    public List<(int x, int y)> GetEmptyCells()
    {
        return Enumerable.Range(0, Width)
            .SelectMany
            (
                x => Enumerable.Range(0, Height).Select(y => (x, y))
            )
            .Where(cell => _field[cell.x, cell.y] == null)
            .ToList();
    }

    public void SyncAllPrevious()
    {
        foreach (Tile t in _tiles)
        {
            t.SyncPrevious();
        }
    }

    public bool TryFindTile(int id, out Tile? foundTile)
    {
        foundTile = _tiles.FirstOrDefault(t => t.Id == id);
        return foundTile != null;
    }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Tile? temp = _field[x, y];
                if (temp != null)
                {
                    sb.Append($"|{temp.ToString(),4}"); // syntax $"{value, num}" provides a format of string to keep grid straight
                }
                else sb.Append($"|{'.', 4}");
            }
            sb.Append("|\n");
        }
        return sb.ToString();
    }
}
