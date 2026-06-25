using Game2048.Core.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Game2048.Core.Models;

public class TileSnapshot
{
    public int Id { get;}

    [JsonConverter(typeof(NullableIntTupleConverter))]
    public (int Id1, int Id2)? Parents { get; }
    public int PosX { get;}
    public int PosY { get;}
    public int Value { get;}

    public TileSnapshot(Tile t)
    {
        if (t == null) throw new ArgumentNullException(nameof(t));

        Id = t.Id;
        Parents = t.Parents;
        PosX = t.PosX;
        PosY = t.PosY;
        Value = t.Value;
    }

    [JsonConstructor]
    public TileSnapshot(int id, int posX, int posY, int value, (int, int)? parents = null)
    {
        Id = id;
        Parents = parents;
        PosX = posX;
        PosY = posY;
        Value = value;
    }
}
