using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048;

internal class TileRegistry : ITileRegistry
{
    private readonly Dictionary<int, Tile> _registry = new();
    public int Count => _registry.Count;
    public Tile? this[int idx] => _registry.GetValueOrDefault(idx);
    
    public void Register(Tile tile)
    {
        ArgumentNullException.ThrowIfNull(tile);
        _registry[tile.Id] = tile;
    }

    public void RegisterMany(IEnumerable<Tile> tiles)
    {
        ArgumentNullException.ThrowIfNull(tiles);
        foreach (Tile tile in tiles) 
        {
            this.Register(tile);
        }
    }

    public void Unregister(int id) 
    {
        if (id < 0)
            throw new ArgumentException("negative id provided");
        _registry.Remove(id); //Remove() will check existence of element no need for if
    }

    public void UnregisterMany(IEnumerable<Tile> tiles)
    {
        ArgumentNullException.ThrowIfNull(tiles);
        foreach (Tile tile in tiles) 
        {
            if(tile is not null)
            {
                this.Unregister(tile.Id);
            }
        }
    }

    public void Clear()
    {
        _registry.Clear();
    }
}
