namespace Game2048.Core.Models;

public class Grid
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    private List<Tile> _tiles;

    private Tile?[,] _field;

    public int Score { get; set; }

    //Count of list of tiles (for cycles)
    public int Count => _tiles.Count;

    //Constructor for empty grid
    public Grid(int width, int height)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Grid cannot be 0x0 or with negative size");

        Width = width;
        Height = height;
        _field = new Tile?[width, height];
        _tiles = new List<Tile>();
        Score = 0;
    }

    //restore from snapshot
    public void Restore(StateSnapshot ss)
    {
        if (ss.Width != Width || ss.Height != Height)
            throw new ArgumentException($"Size of the snapshot ({ss.Width}x{ss.Height}) does not fit the grid ({Width}x{Height})");

        Dictionary<int, Tile> currentTiles = _tiles.ToDictionary(t => t.Id);
        Dictionary<int, Tile> restoredTiles = new Dictionary<int, Tile>();

        Array.Clear(_field, 0, _field.Length);
        _tiles.Clear();
        Score = ss.Score;

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
            if (!t.Parents.HasValue) throw new ArgumentException("Parents cannot be found in snapshot's merged tiles");
            if (!restoredTiles.ContainsKey(t.Parents.Value.Id1) || !restoredTiles.ContainsKey(t.Parents.Value.Id2)) throw new InvalidOperationException($"No parents for id:{t.Id} found in current game state");
            restoredTiles[t.Parents.Value.Id1].SetPrevious(t.PosX, t.PosY);
            restoredTiles[t.Parents.Value.Id2].SetPrevious(t.PosX, t.PosY);
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
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                throw new IndexOutOfRangeException($"coordinates {x} or {y} provided aren't correct");

            if (value != null && _tiles.Contains(value))
                throw new InvalidOperationException("Cannot set object that already exists in grid");

            if (_field[x, y] != null)
            {
                _tiles.Remove(_field[x, y]!);
            }

            _field[x, y] = value;

            if(value != null)
            {
                _tiles.Add(value);
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
        if (y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException("y is out of range");

        Tile?[] row = new Tile?[Width];
        for (int x = 0; x < Width; x++)
        {
            row[x] = _field[x, y];
        }
        return row;
    }

    public void SetRow(int y, Tile?[] row)
    {
        ArgumentNullException.ThrowIfNull(row);
        if (row.Length != Width) 
            throw new ArgumentException("Inapropriate array length");
        if (y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException("x is out of range");

        for (int x = 0; x < Width; x++) 
        {
            Tile? oldTile = _field[x, y];
            Tile? newTile = row[x];

            if (!row.Contains(oldTile)) 
            {
                _tiles.Remove(oldTile!);
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
        if(x < 0 || x >= Width) 
            throw new ArgumentOutOfRangeException("x is out of range");

        Tile?[] column = new Tile?[Height];
        for (int y = 0; y < Height; y++) 
        {
            column[y] = _field[x, y];
        }
        return column;
    }

    public void SetColumn(int x, Tile?[] column)
    {
        ArgumentNullException.ThrowIfNull(column);
        if (column.Length != Height)
            throw new ArgumentException("Inapropriate array length");
        if (x < 0 || x >= Width)
            throw new ArgumentOutOfRangeException("x is out of range");

        for (int y = 0; y < Height; y++) 
        {
            Tile? oldTile = _field[x, y];
            Tile? newTile = column[y];

            if (!column.Contains(oldTile))
            {
                _tiles.Remove(oldTile!);
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

    public StateSnapshot CreateSnapshot(int nextId) 
    {
        if (nextId < 1)
            throw new ArgumentException("Invalid id counter provided");
        return new StateSnapshot(Width, Height, Score, _tiles, nextId);
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

    public bool CheckForValue(int valueToCheck)
    {
        if (
            !(
                valueToCheck > 0 
                && 
                (valueToCheck & valueToCheck - 1) == 0 //check if value is a power of 2
            )
           )
            throw new ArgumentException("The value is invalid or not a power of 2");

        foreach (Tile t in _tiles)
        {
            if (t.Value == valueToCheck) return true;
        }
        return false;
    }
}
