using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048;

internal interface IReadOnlyTileRegistry
{
    Tile? this[int idx] { get; }
}
