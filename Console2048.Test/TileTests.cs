using Console2048;
using Xunit;

namespace Console2048.Test;

public class TileTests
{
    [Fact]
    public void Constructor_SetData_Correctly()
    {
        Tile tile = new Tile(id: 1, posX: 2, posY: 3, previousX: 2, previousY: 4, isMerged: true, value: 8);

        Assert.Equal(1, tile.Id);
        Assert.Equal(2, tile.PosX);
        Assert.Equal(3, tile.PosY);
        Assert.Equal(2, tile.PreviousX);
        Assert.Equal(4, tile.PreviousY);
        Assert.True(tile.IsMerged);
        Assert.Equal(8, tile.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-55)]
    public void TileInit_WithNegative_Parameters_Throws_ArgumentException(int x)
    {
        Assert.Throws<ArgumentException>(() => new Tile(x, 0, 0, 0, 0, false, 2));
        Assert.Throws<ArgumentException>(() => new Tile(0, x, 0, 0, 0, false, 2));
        Assert.Throws<ArgumentException>(() => new Tile(0, 0, x, 0, 0, false, 2));
        Assert.Throws<ArgumentException>(() => new Tile(0, 0, 0, x, 0, false, 2));
        Assert.Throws<ArgumentException>(() => new Tile(0, 0, 0, 0, x, false, 2));
        Assert.Throws<ArgumentException>(() => new Tile(0, 0, 0, 0, 0, false, x));
    }

    [Fact]
    public void PositionCan_BeChanged_Properly()
    {
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2);
        tile.SetPosition(2, 0);
        tile.SetPosition(2, 2);

        Assert.Equal((2, 2), (tile.PosX, tile.PosY));
    }

    [Fact]
    public void PreviousCan_BeChanged_Properly()
    {
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2);
        tile.SetPrevious(2, 0);
        tile.SetPrevious(2, 2);

        Assert.Equal((2, 2), (tile.PreviousX, tile.PreviousY));
    }

    [Fact]
    public void ValueCan_BeChanged()
    {
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2);
        tile.SetValue(4);

        Assert.Equal(4, tile.Value);
    }

    [Theory]
    [InlineData (-1, 0)]
    [InlineData (0, -1)]
    [InlineData (-1, -2)]
    [InlineData (-55, -44)]
    public void NegativePosition_SetThrows_ArgumentException(int x, int y)
    {
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2);
        Assert.Throws<ArgumentException>(() => tile.SetPosition(x, y));
        Assert.Throws<ArgumentException>(() => tile.SetPrevious(x, y));
    }

    [Fact]
    public void IsMerged_FlagCan_BeChanged()
    {
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2);
        tile.SetMerged(true);

        Assert.True(tile.IsMerged);
    }



    [Fact]
    public void ParentsShould_BeNull_WhenInit()
    {
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2);

        Assert.Null(tile.Parents);
    }

    [Fact]
    public void SetParents_StoresParents_IdsProvided()
    {
        Tile tile = new Tile(3, 0, 0, 0, 0, true, 4);
        tile.SetParents(1, 2);

        Assert.Equal((1,2), (tile.Parents!.Value.Id1, tile.Parents.Value.Id2));
    }

    [Fact]
    public void SetParents_WithNo_ArgsSets_Null()
    {
        Tile tile = new Tile(3, 0, 0, 0, 0, true, 4);
        tile.SetParents(1, 2);
        tile.SetParents();

        Assert.Null(tile.Parents);
    }

    [Fact]
    public void SyncPrevious_MakesPrevious_SameAs_Position()
    {
        Tile tile = new Tile(1, 2, 2, 0, 2, false, 2);
        tile.SyncPrevious();

        Assert.Equal(2, tile.PreviousX);
    }

    [Fact]
    public void UpdatePosition_ChangesPosition_ButKeepPrevious()
    {
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2);
        tile.UpdatePosition(0, 2);

        Assert.Equal(2, tile.PosY);
        Assert.Equal(0, tile.PreviousY);
    }

    [Fact]
    public void ToString_Returns_ValueString()
    {
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2048);
        Assert.Equal("2048", tile.ToString());
    }


}