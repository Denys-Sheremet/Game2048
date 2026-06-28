using System.Text.Json.Serialization;

namespace Game2048.Core.Models;

public class StateSnapshot
{
    public int Width { get; }
    public int Height { get; }
    public IReadOnlyList<TileSnapshot> TileSnapshots {  get; } //do readonly to prevent pointer leak
    public int Score { get; }
    public int NextId { get; }


    public StateSnapshot(int width, int height, int score, IEnumerable<Tile> tiles, int nextId)
    {
        ArgumentNullException.ThrowIfNull(tiles);
        Width = width;
        Height = height;
        Score = score;
        NextId = nextId;

        TileSnapshots = tiles
            .Select(t => new TileSnapshot(t))
            .ToList()
            .AsReadOnly();
    }

    [JsonConstructor]
    public StateSnapshot(int width, int height, int score, IEnumerable<TileSnapshot> tileSnapshots, int nextId)
    {
        ArgumentNullException.ThrowIfNull(tileSnapshots);
        Width = width;
        Height = height;
        Score = score;
        NextId = nextId;
        TileSnapshots = tileSnapshots.ToList().AsReadOnly();
    }
}
