using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048;

internal interface ITileRegistry : IReadOnlyTileRegistry
{
    void Register(Tile tile);
    void RegisterMany(IEnumerable<Tile> tiles);
    void Unregister(int id);
    void UnregisterMany(IEnumerable<Tile> tiles);
    void Clear();
}
