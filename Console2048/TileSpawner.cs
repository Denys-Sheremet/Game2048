using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048;

internal class TileSpawner : ITileSpawner
{
    public bool Spawn(Grid grid, ITileRegistry registry, int nextId, IRandomProvider random)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(random);
        if (nextId < 0) throw new ArgumentException($"Invalid id provided: {nextId}");

        List<(int, int)> emptyCells = grid.GetEmptyCells();
        if (emptyCells.Count < 1)
        {
            return false;
        }
        (int x, int y) chosen = emptyCells[random.Next(emptyCells.Count)];

        int value = random.Next(10) == 0 ? 4 : 2; //10% that 4 will appear
        Tile newTile = new Tile(nextId, chosen.x, chosen.y, value);

        grid[chosen.x, chosen.y] = newTile;
        registry.Register(newTile);

        return true;
    }
    public bool TrySpawnAt
        (Grid grid, ITileRegistry registry, int nextId, int x, int y, int? value = null, IRandomProvider? random = null)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(registry);
        if (nextId < 0) 
            throw new ArgumentException($"Invalid id provided: {nextId}");

        if (x < 0 || y < 0 || x >= grid.Width || y >= grid.Height)
            throw new ArgumentException("Invalid coordinates provided to spawn");
        if (grid[x, y] is not null)
        {
            return false;
        }
        Tile newTile;
        int newValue;
        if (value == null)
        {
            if (random == null) 
                throw new ArgumentNullException("RandomProvider should be provided if no value");
            newValue = random.Next(10) == 0 ? 4 : 2;
        }
        else
        {
            if (value % 2 != 0 || value < 2)
                throw new ArgumentException("Invalid value for tile provided");
            newValue = (int)value;
        }
        newTile = new Tile(nextId, x, y, newValue);

        grid[x, y] = newTile;
        registry.Register(newTile);

        return true;
    }
}
