using Game2048.Core.Mechanics;
using Game2048.Core.Models;
using Game2048.Core.Services;
using Game2048.Core.Interfaces;
using Xunit;

namespace Game2048.Core.Test;

public class TileSnapshotTest
{
    [Fact]
    public void TileSnapshot_Constructor_SavesAll_Important_Data()
    {
        Tile tile = new Tile(3, 2, 3, 2, 3, true, 4);
        tile.SetParents(1, 2);

        TileSnapshot snapshot = new TileSnapshot(tile);

        Assert.Equal(3, snapshot.Id);
        Assert.Equal((2, 3), (snapshot.PosX, snapshot.PosY));
        Assert.NotNull(snapshot.Parents);
        Assert.Equal((1, 2), (snapshot.Parents.Value.Id1, snapshot.Parents.Value.Id2));
        Assert.Equal(4, snapshot.Value);
    }

    [Fact]
    public void ParentsAre_NullWhen_TileDoes_NotHave_It()
    {
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2);

        TileSnapshot snapshot = new TileSnapshot(tile);

        Assert.Null(snapshot.Parents);
    }

    [Fact]
    public void ConstructorThrows_ArgumentNullException_WhenTile_IsNull()
    {
        Tile? nullTile = null;
        Assert.Throws<ArgumentNullException>(() => new TileSnapshot(nullTile!));
    }

    [Fact]
    public void TileSnapshot_IsImmutable_AndRepresents_RawData()
    {
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2);
        TileSnapshot snapshot = new TileSnapshot(tile);

        tile.SetPosition(2, 0);
        tile.SetPosition(2, 2);

        tile.SetPrevious(2, 2);
        tile.SetValue(4);
        tile.SetMerged(true);
        tile.SetParents(1, 2);

        Assert.Equal((0, 0), (snapshot.PosX, snapshot.PosY));
        Assert.Null(snapshot.Parents);
        Assert.Equal(2, snapshot.Value);
    }
}
