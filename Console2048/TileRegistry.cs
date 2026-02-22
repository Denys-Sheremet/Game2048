using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048;

internal class TileRegistry : IReadOnlyTileRegistry
{
    private readonly Dictionary<int, Tile> _registry = new();
    public int Count => _registry.Count;
    public Tile? this[int idx] => _registry.GetValueOrDefault(idx);

    public void Register(Tile tile)
    {
        _registry[tile.Id] = tile;
    }

    public void RegisterMany(IEnumerable<Tile> tiles)
    {
        foreach (Tile tile in tiles) 
        {
            this.Register(tile);
        }
    }

    public void Unregister(int id) 
    {
        _registry.Remove(id); //Remove() will check existence of element no need for if
    }

    public void UnregisterMany(IEnumerable<Tile> tiles)
    {
        foreach (Tile tile in tiles) 
        {
            this.Unregister(tile.Id);
        }
    }

    public void Clear()
    {
        _registry.Clear();
    }
}
