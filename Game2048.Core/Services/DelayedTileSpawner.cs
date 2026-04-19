using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Core.Services;

public class DelayedTileSpawner : TileSpawner, ITileSpawner
{
    private readonly int _interval;
    private int _initialSpawnsLeft;
    public int MovesLeft { get; private set; }
    public DelayedTileSpawner(int interval, int initialSpawnsLeft = 2)
    {
        if (interval <= 0) throw new ArgumentException("Interval cannot be negative or 0");
        _interval = interval;
        MovesLeft = interval;
        _initialSpawnsLeft = initialSpawnsLeft;
    }

    public override bool Spawn(Grid grid, ITileRegistry registry, int nextId, IRandomProvider random)
    {
        if (_initialSpawnsLeft > 0)
        {
            bool spawned = base.Spawn(grid, registry, nextId, random);
            if (spawned)
            {
                _initialSpawnsLeft--;
            }
            return spawned;
        }

        MovesLeft--;

        if (MovesLeft > 0) return false;

        bool success = base.Spawn(grid, registry, nextId, random);

        if (success) 
        {
            MovesLeft = _interval;
        }
        else
        {
            MovesLeft = 0;
        }
        return success;
    }

    public void Reset(int resettedInitialSpawns = 2)
    {
        MovesLeft = _interval;
        _initialSpawnsLeft = resettedInitialSpawns;
    }
}
