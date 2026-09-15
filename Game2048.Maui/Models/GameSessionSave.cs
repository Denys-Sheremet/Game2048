using Game2048.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.Models;

public class GameSessionSave
{
    public required StateSnapshot LastState { get; set; }
    public List<StateSnapshot>? History { get; set; }
    public int? MaxTileValue { get; set; }
}
