using Console2048;
using Xunit;

namespace Console2048.Test;

public class GridTests
{
    [Fact]
    public void ConstructorCreates_GridWith_CorrectProperties()
    {
        Grid grid = new Grid(width: 4, height: 4);

        Assert.Equal((4, 4), (grid.Width, grid.Height));
        Assert.Equal(0, grid.Score);
    }

    [Fact]
    public void GetCount_ReturnsCorrect_AmountOf_TilesStored()
    {
        Grid grid = new Grid(4, 4);
        grid[2, 2] = new Tile(1, 0, 0, 0, 0, false, 2);
        grid[1, 1] = new Tile(2, 1, 1, 0, 0, false, 2);

        int countOfTiles = grid.GetCount();

        Assert.Equal(2, countOfTiles);
    }

    [Fact]
    public void InsertionOf_TileWith_IndexerSync_BothField_AndList_OfTiles()
    {
        Grid grid = new Grid(4, 4);
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2);
        grid[0, 0] = tile;

        Assert.NotNull(grid[0, 0]);
        Assert.Equal(1, grid.GetCount());
        Assert.Equal(tile, grid[0]);
    }

    [Fact]
    public void SnapshotStores_AllImportant_Information_ToRestore()
    {
        Grid grid = new Grid(4, 4);
        Tile tile1 = new Tile(1, 2, 2, 0, 0, false, 2);
        Tile tile2 = new Tile(2, 1, 1, 0, 0, false, 2);
        grid[2, 2] = tile1;
        grid[1, 1] = tile2;

        StateSnapshot ss = grid.CreateSnapshot();

        Assert.NotNull(ss);
        Assert.Equal((4, 4, 0), (ss.Width, ss.Height, ss.Score));
        Assert.Equal(2, ss.TileSnapshots.Count);
        Assert.True(ss.TileSnapshots.Where(t => t.Id == 1).Any());

    }

    [Fact]
    public void GetEmptyCells_Returns_CorrectAmount_OfEmpty_Cells_InGrid()
    {
        Grid grid = new Grid(2, 2);
        grid[0, 0] = new Tile(1, 0, 0, 0, 0, false, 2);
        grid[1, 1] = new Tile(2, 1, 1, 0, 0, false, 2);

        List<(int x, int y)> emptyCells = grid.GetEmptyCells();

        Assert.Equal(2, emptyCells.Count);
    }

    [Fact]
    public void TryFindTile_ReturnsTrue_IfTile_IsPresent_InGrid()
    {
        Grid grid = new Grid(4, 4);
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2);

        grid[0, 0] = tile;

        bool isFound = grid.TryFindTile(id: 1, out Tile? result);

        Assert.True(isFound);
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void TryFindTile_Returns_NullIf_TileIsMissing()
    {
        Grid grid = new Grid(4, 4);

        bool isFound = grid.TryFindTile(id: 1, out Tile? result);

        Assert.False(isFound);
        Assert.Null(result);
    }
}
