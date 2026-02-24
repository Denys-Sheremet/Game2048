using Console2048;
using Xunit;

namespace Console2048.Test;

public class GameTest
{
    [Fact]
    public void ConstructorInitialize_Correct_Data()
    {
        Grid grid = new Grid(4, 4);
        Game game = new Game(grid);

        Assert.Equal(game.Grid, grid);
        Assert.True(game.HistoryIsEmpty());
        Assert.NotNull(game.TileRegistry);
        Assert.False(game.IsGameOver);
        Assert.Equal(1, game.GetNextTileId());
    }

    [Fact]
    public void ConstructorThrows_ArgumentNullException_IfGrid_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Game(null!));
    }

    [Fact]
    public void SpawnNewTile_SpawnsOne_TilePer_Call()
    {
        Grid grid = new Grid(4, 4);
        Game game = new Game(grid);

        game.SpawnNewTile();

        Assert.Equal(1, game.Grid.Count);
    }

    [Fact]
    public void SpawnNewTile_Creates_NewTile_OnRandom_Position()
    {
        Grid grid = new Grid(4, 4);
        Game game = new Game(grid);
        game.SpawnNewTile();
        game.SpawnNewTile();

        game.Grid.TryFindTile(1, out Tile? tile1);
        game.Grid.TryFindTile(2, out Tile? tile2);
        Assert.NotNull(tile1);
        Assert.NotNull(tile2);
    }

    [Fact]
    public void SpawnMultipleTiles_MethodCan_SpawnMany_NewTiles_ByCount_Provided()
    {
        Grid grid = new Grid(4, 4);
        Game game = new Game(grid);

        game.SpawnMultipleTiles(3);

        Assert.Equal(3, game.Grid.Count);
        game.Grid.TryFindTile(1, out Tile? tile1);
        Assert.NotNull(tile1);
    }

    [Fact]
    public void WhenSpawn_NewTiles_ThereAre_NoCollisions_ByPosition()
    {
        Grid grid = new Grid(2, 2);
        Game game = new Game(grid);

        game.SpawnMultipleTiles(2);
        game.Grid.TryFindTile(1, out Tile? tile1);
        game.Grid.TryFindTile(2, out Tile? tile2);

        Assert.False(tile1!.PosX == tile2!.PosX && tile1!.PosY == tile2!.PosY);
    }

    [Fact]
    public void SpawnNewTile_WhenGridIsFull_TriggersGameOverState()
    {
        Grid grid = new Grid(1, 1);
        Game game = new Game(grid);
        game.SpawnNewTile(); //now grid is full

        game.SpawnNewTile();
        Assert.True(game.IsGameOver);
    }

    [Fact]
    public void IdCounter_ReturnsNew_UniqueIds_AndIncrement_WhenSpawn_NewTiles()
    {
        Grid grid = new Grid(4, 4);
        Game game = new Game(grid);

        int count = 5;
        game.SpawnMultipleTiles(count);

        for (int i = 1; i <= count; i++) 
        {
            game.Grid.TryFindTile(i, out Tile? tile);
            Assert.NotNull(tile);
        }
    }

    [Fact]
    public void IfThereIs_NoEmpty_Cell_SpawnNewTile_Will_Not_Crash_TheProgram()
    {
        Grid grid = new Grid(1, 1);
        Game game = new Game(grid);

        Exception exception = Record.Exception(() => game.SpawnMultipleTiles(5));

        Assert.Null(exception);
    }

    [Fact]
    public void MoveMethod_MovesTile_OnGridCorrectly()
    {
        Grid grid = new Grid(4, 4);
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2);
        grid[0, 0] = tile;
        Game game = new Game(grid);

        game.Move(MoveDirection.Right, false);
        Assert.Null(game.Grid[0, 0]);
        Assert.Equal(tile, game.Grid[3, 0]);

        game.Move(MoveDirection.Down, false);
        Assert.Null(game.Grid[3, 0]);
        Assert.Equal(tile, game.Grid[3, 3]);

        game.Move(MoveDirection.Left, false);
        Assert.Null(game.Grid[3, 3]);
        Assert.Equal(tile, game.Grid[0, 3]);

        game.Move(MoveDirection.Up, false);
        Assert.Null(game.Grid[0, 3]);
        Assert.Equal(tile, game.Grid[0, 0]);
    }

    [Theory]
    [InlineData(MoveDirection.Left, new[] { 0, 0 }, new[] { 1, 0 }, new[] { 0, 0 })]
    [InlineData(MoveDirection.Right, new[] { 0, 0 }, new[] { 1, 0 }, new[] { 3, 0 })]
    [InlineData(MoveDirection.Up, new[] { 0, 0 }, new[] { 0, 1 }, new[] { 0, 0 })]
    [InlineData(MoveDirection.Down, new[] { 0, 0 }, new[] { 0, 1 }, new[] { 0, 3 })]
    [InlineData(MoveDirection.Left, new[] { 1, 2 }, new[] { 0, 2 }, new[] { 0, 2 })]
    [InlineData(MoveDirection.Right, new[] { 3, 3 }, new[] { 1, 3 }, new[] { 3, 3 })]
    [InlineData(MoveDirection.Up, new[] { 2, 3 }, new[] { 2, 1 }, new[] { 2, 0 })]
    [InlineData(MoveDirection.Down, new[] { 2, 1 }, new[] { 2, 2 }, new[] { 2, 3 })]
    public void MoveMethod_MergesTiles_CorrectlyInEvery_Direction
        (MoveDirection direction, int[] parent1Pos, int[] parent2Pos, int[] mergedPos)
    {
        Grid grid = new Grid(4, 4);
        Tile tile1 = new Tile(1, parent1Pos[0], parent1Pos[1], parent1Pos[0], parent1Pos[1], false, 2);
        Tile tile2 = new Tile(2, parent2Pos[0], parent2Pos[1], parent2Pos[0], parent2Pos[1], false, 2);
        grid[parent1Pos[0], parent1Pos[1]] = tile1;
        grid[parent2Pos[0], parent2Pos[1]] = tile2;

        Game game = new Game(grid);

        game.Move(direction, false);

        Tile merged = game.Grid[0];

        Assert.Equal(1, game.Grid.Count);
        Assert.True(merged.Parents.HasValue);
        bool hasId1 = new[] 
        {
            merged.Parents!.Value.Id1,
            merged.Parents!.Value.Id2 
        }
        .Any(id => id == 1);
        
        bool hasId2 = new[] 
        {
            merged.Parents!.Value.Id1,
            merged.Parents!.Value.Id2 
        }
        .Any(id => id == 2);
        
        Assert.True(hasId1);
        Assert.True(hasId2);
        Assert.Equal((mergedPos[0], mergedPos[1]), (merged.PosX, merged.PosY));
        Assert.Equal(4, merged.Value);
        Assert.Equal(4, game.Grid.Score);
    }

    [Fact]
    public void Move_Method_SetsPrevious_Coordinates_ByPrevious_State()
    {
        Grid grid = new Grid(4, 4);
        Tile tile1 = new Tile(1, 0, 0, 2);
        Tile tile2 = new Tile(2, 3, 3, 2);
        grid[0, 0] = tile1;
        grid[3, 3] = tile2;

        Game game = new Game(grid);

        game.Move(MoveDirection.Right, false);

        Assert.Equal(0, tile1.PreviousX);
        Assert.Equal(0, tile1.PreviousY);
        Assert.Equal(3, tile1.PosX);
        Assert.Equal(0, tile1.PosY);
        Assert.Equal(3, tile2.PreviousX);
        Assert.Equal(3, tile2.PreviousY);
        Assert.Equal(3, tile2.PosX);
        Assert.Equal(3, tile2.PosY);
    }
    
    [Fact]
    public void Every_Move_HistoryIncreases_ByOne_State()
    {
        Grid grid = new Grid(4, 4);
        grid[0, 0] = new Tile(1, 0, 0, 2);

        Game game = new Game(grid);

        game.Move(MoveDirection.Right, false);

        Assert.Equal(1, game.HistoryCount);
    }
}
