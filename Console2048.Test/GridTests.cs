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
    public void Constructor_Throws_ArgumentException_IfSize_IsIncorrect()
    {
        Grid grid;
        Assert.Throws<ArgumentException>(() => grid = new Grid(-1, 1));
        Assert.Throws<ArgumentException>(() => grid = new Grid(1, -1));
        Assert.Throws<ArgumentException>(() => grid = new Grid(0, 0));
    }

    [Fact]
    public void RestoreMethod_RestoreFull_GridProperly()
    {
        Grid grid = new Grid(4, 4);
        grid.Score = 4;
        grid[0, 0] = new Tile(1, 0, 0, 0, 0, false, 2);
        grid[1, 1] = new Tile(2, 1, 1, 0, 0, false, 2);
        grid[2, 2] = new Tile(3, 2, 2, 0, 0, false, 2);

        Tile tile = new Tile(6, 3, 3, 0, 0, false, 4);//false here because true will force Restore() to find parents in the state
        tile.SetParents(4, 5);
        grid[3, 3] = tile;

        StateSnapshot ss = grid.CreateSnapshot();

        //add extra tile to make difference between old grid and new one
        grid[0, 1] = new Tile(7, 0, 1, 0, 0, false, 2);

        grid.Restore(ss);

        Assert.Equal(4, grid.Score);
        Assert.Equal(4, grid.GetCount());
        Assert.True(grid[3, 3].Parents.HasValue);
        Assert.Equal((4, 5), (grid[3, 3].Parents.Value.Id1, grid[3, 3].Parents.Value.Id2));
        Assert.True(grid.TryFindTile(1, out Tile? foundTile));
        Assert.NotNull(foundTile);
    }

    [Fact]
    public void GetRow_ReturnsExact_Row()
    {
        Grid grid = new Grid(4, 4);
        grid[0, 0] = new Tile(1, 0, 0, 0, 0, false, 2);
        grid[1, 0] = new Tile(2, 1, 0, 0, 0, false, 2);
        grid[2, 0] = new Tile(3, 2, 0, 0, 0, false, 2);
        grid[3, 1] = new Tile(4, 3, 1, 0, 0, false, 2); //its other row

        Tile?[] row = grid.GetRow(0);

        Assert.Equal(row[0], grid[0, 0]);
        Assert.Equal(row[1], grid[1, 0]);
        Assert.Equal(row[2], grid[2, 0]);
        Assert.Null(row[3]);
        Assert.DoesNotContain(grid[3, 1], row);
    }

    [Fact]
    public void GetRow_Throws_ArgumentOutOfRangeException_WhenIndexIsInvalid()
    {
        Grid grid = new Grid(4, 4);

        Assert.Throws<ArgumentOutOfRangeException>(() => grid.GetRow(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => grid.GetRow(4));
    }

    [Fact]
    public void GetColumn_ReturnsExact_Column()
    {
        Grid grid = new Grid(4, 4);
        grid[0, 0] = new Tile(1, 0, 0, 0, 0, false, 2);
        grid[0, 1] = new Tile(2, 0, 1, 0, 0, false, 2);
        grid[0, 2] = new Tile(3, 0, 2, 0, 0, false, 2);
        grid[1, 3] = new Tile(4, 1, 3, 0, 0, false, 2); //its other column

        Tile?[] column = grid.GetColumn(0);

        Assert.Equal(column[0], grid[0, 0]);
        Assert.Equal(column[1], grid[0, 1]);
        Assert.Equal(column[2], grid[0, 2]);
        Assert.Null(column[3]);
        Assert.DoesNotContain(grid[1, 3], column);
    }

    [Fact]
    public void GetColumn_Throws_ArgumentOutOfRangeException_WhenIndexIsInvalid()
    {
        Grid grid = new Grid(4, 4);

        Assert.Throws<ArgumentOutOfRangeException>(() => grid.GetColumn(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => grid.GetColumn(4));
    }

    [Fact]
    public void SetRow_SetsRow_InGrid_Properly()
    {
        Grid grid = new Grid(4, 4);
        grid[0, 0] = new Tile(1, 0, 0, 0, 0, false, 2);

        Tile?[] newRow = new Tile?[4];
        newRow[3] = new Tile(2, 3, 0, 0, 0, false, 2);

        grid.SetRow(0, newRow);

        Assert.Equal(grid[3, 0], newRow[3]);
        Assert.Null(grid[0, 0]);
        Assert.Equal(1, grid.GetCount());
    }

    [Fact]
    public void SetRow_Throws_ArgumentException_WhenArrayLength_IsIncorrect()
    {
        Grid grid = new Grid(4, 4);
        Tile?[] smallRow = new Tile?[2];
        Tile?[] bigRow = new Tile?[5];

        Assert.Throws<ArgumentException>(() => grid.SetRow(0, smallRow));
        Assert.Throws<ArgumentException>(() => grid.SetRow(0, bigRow));
    }

    [Fact]
    public void SetRow_Throws_ArgumentOutOfRangeException_WhenCoordinates_AreOutOfRange()
    {
        Grid grid = new Grid(4, 4);
        Tile?[] row = new Tile?[4];

        Assert.Throws<ArgumentOutOfRangeException>(() => grid.SetRow(-1, row));
        Assert.Throws<ArgumentOutOfRangeException>(() => grid.SetRow(4, row));
    }

    [Fact]
    public void SetColumn_SetsColumn_InGrid_Properly()
    {
        Grid grid = new Grid(4, 4);
        grid[0, 0] = new Tile(1, 0, 0, 0, 0, false, 2);

        Tile?[] newColumn = new Tile?[4];
        newColumn[3] = new Tile(2, 0, 3, 0, 0, false, 2);

        grid.SetColumn(0, newColumn);

        Assert.Equal(grid[0, 3], newColumn[3]);
        Assert.Null(grid[0, 0]);
        Assert.Equal(1, grid.GetCount());
    }

    [Fact]
    public void SetColumn_Throws_ArgumentException_WhenArrayLength_IsIncorrect()
    {
        Grid grid = new Grid(4, 4);
        Tile?[] smallColumn = new Tile?[2];
        Tile?[] bigColumn = new Tile?[5];

        Assert.Throws<ArgumentException>(() => grid.SetColumn(0, smallColumn));
        Assert.Throws<ArgumentException>(() => grid.SetColumn(0, bigColumn));
    }

    [Fact]
    public void SetColumn_Throws_ArgumentOutOfRangeException_WhenCoordinates_AreOutOfRange()
    {
        Grid grid = new Grid(4, 4);
        Tile?[] column = new Tile?[4];

        Assert.Throws<ArgumentOutOfRangeException>(() => grid.SetColumn(-1, column));
        Assert.Throws<ArgumentOutOfRangeException>(() => grid.SetColumn(4, column));
    }

    [Fact]
    public void SyncAllPrevious_SyncAll_TilesIn_Grid()
    {
        Grid grid = new Grid(2, 2);
        grid[1, 1] = new Tile(1, 1, 1, previousX: 0, previousY: 0, false, 2);
        grid[0, 0] = new Tile(1, 0, 0, previousX: 1, previousY: 0, false, 2);

        grid.SyncAllPrevious();

        Assert.Equal((1, 1), (grid[1, 1]!.PreviousX, grid[1, 1]!.PreviousY));
        Assert.Equal((0, 0), (grid[0, 0]!.PreviousX, grid[0, 0]!.PreviousY));
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

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(4, 0)]
    [InlineData(0, -1)]
    [InlineData(0, 4)]
    public void Indexer_Throws_IndexOutOfRangeException_OnInvalidCoordinates(int x, int y)
    {
        Grid grid = new Grid(4, 4);
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2);

        Assert.Throws<IndexOutOfRangeException>(() => grid[x, y]);
        Assert.Throws<IndexOutOfRangeException>(() => grid[x, y] = tile);
    }

    [Fact]
    public void ChangeOf_TileInPlace_DoesNot_Affect_TilesCount()
    {
        Grid grid = new Grid(4, 4);
        grid[0, 0] = new Tile(1, 0, 0, 0, 0, false, 2);
        grid[0, 0] = new Tile(2, 0, 0, 0, 0, false, 4);

        Assert.Equal(1, grid.GetCount());
    }

    [Fact]
    public void ChangeExisting_TileIn_GridSets_ErazeIt_And_Decrease_TilesCount()
    {
        Grid grid = new Grid(4, 4);
        grid[0, 0] = new Tile(1, 0, 0, 0, 0, false, 2);
        grid[0, 0] = null;

        Assert.Equal(0, grid.GetCount());
    }

    [Fact]
    public void TryingTo_SetTile_ThatIs_AlreadyExists_Throws_InvalidOperationException()
    {
        Grid grid = new Grid(4, 4);
        grid[0, 0] = new Tile(1, 0, 0, 0, 0, false, 2);
        
        Assert.Throws<InvalidOperationException>(() => grid[1, 1] = grid[0, 0]);
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
