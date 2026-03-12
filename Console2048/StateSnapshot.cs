using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048;

public class StateSnapshot
{
    public int Width { get; }
    public int Height { get; }
    public IReadOnlyList<TileSnapshot> TileSnapshots {  get; } //do readonly to prevent pointer leak
    public int Score { get; }
    public int NextId { get; }

    public StateSnapshot(int width, int height, int score, IEnumerable<Tile> tiles, int nextId)
    {
        Width = width;
        Height = height;
        Score = score;
        NextId = nextId;

        TileSnapshots = tiles
            .Select(t => new TileSnapshot(t))
            .ToList()
            .AsReadOnly();
     }

    //Debug
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var t in TileSnapshots)
        {
            sb.AppendLine(t.ToString());
        }
        return sb.ToString();
    }
}
