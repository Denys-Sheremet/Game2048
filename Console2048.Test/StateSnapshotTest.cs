using Console2048;
using Xunit;

namespace Console2048.Test;

public class StateSnapshotTest
{
    [Fact]
    public void StateSnapshot_StoresRawData_AboutGame_StateAt_TheMoment()
    {
        Grid grid = new Grid(2, 3);
        grid[0, 0] = new Tile(1, 0, 0, 0, 0, false, 2);
        grid[1, 0] = new Tile(2, 1, 0, 0, 0, false, 4);
        Tile tile = new Tile(3, 1, 2, 0, 0, true, 4);
        tile.SetParents(55, 66);
        grid[1, 2] = tile;

        StateSnapshot snapshot = grid.CreateSnapshot();

        Assert.Equal((grid.Width, grid.Height), (snapshot.Width, snapshot.Height));
        Assert.Equal(grid.Score, snapshot.Score);
        Assert.Equal(grid.GetCount(), snapshot.TileSnapshots.Count);


        for (int i = 0; i < snapshot.TileSnapshots.Count; i++)
        {
            Assert.Equal(grid[i].Id, snapshot.TileSnapshots[i].Id);
            Assert.Equal(grid[i].PosX, snapshot.TileSnapshots[i].PosX);
            Assert.Equal(grid[i].PosY, snapshot.TileSnapshots[i].PosY);
            Assert.Equal(grid[i].Value, snapshot.TileSnapshots[i].Value);
            Assert.Equal(grid[i].Parents.HasValue, snapshot.TileSnapshots[i].Parents.HasValue);
        }
    }

    [Fact]
    public void StateSnapshot_IsImmutable_AndDoes_NotSensitive_ToChanges_InState()
    {
        Grid grid = new Grid(2, 2);
        grid[0, 0] = new Tile(1, 0, 0, 0, 0, false, 2);
        grid[1, 1] = new Tile(2, 1, 1, 1, 1, false, 4);

        StateSnapshot ss = grid.CreateSnapshot();

        grid[0, 0] = null;
        grid[1, 0] = new Tile(3, 1, 0, 0, 0, false, 8);
        grid[0, 1] = new Tile(4, 0, 1, 0, 0, false, 2);

        Assert.True(ss.TileSnapshots.Where(s => s.Id == 1).Any());
        Assert.False(ss.TileSnapshots.Where(s => s.Id == 3).Any());
        Assert.Equal(2, ss.TileSnapshots.Count);
    }
}
