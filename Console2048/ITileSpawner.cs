using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048;

public interface ITileSpawner
{
    bool Spawn(Grid grid, ITileRegistry registry, int nextId, IRandomProvider random);
    bool TrySpawnAt(Grid grid, ITileRegistry registry, int nextId, int x, int y, int? value, IRandomProvider? random);
}
