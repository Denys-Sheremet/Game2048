using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048;

internal class TileRegistry
{
    private Dictionary<int, Tile> _registry;

    public TileRegistry()
    {
        _registry = new Dictionary<int, Tile>();
    }

    public Tile? this[int idx]
    {
        get 
        {
            return _registry.GetValueOrDefault(idx);
        }
    }

    public void Register(Tile tile)
    {
        _registry.Add(tile.Id, tile);
    }

    public void Unregister(int id) 
    {
        _registry.Remove(id); //Remove() will check existence of element no need for if
    }
}
