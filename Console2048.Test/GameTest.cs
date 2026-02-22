using Console2048;
using Xunit;

namespace Console2048.Test;

public class GameTest
{
    [Fact]
    public void ConstructorInitialize_Correctly()
    {
        Grid grid = new Grid(4, 4);
        Game game = new Game(grid);

        Assert.Equal(game.Grid, grid);
        Assert.NotNull(game.TileRegistry);
        Assert.False(game.IsGameOver);
    }

    [Fact]
    public void ConstructorThrows_ArgumentNullException_IfGrid_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Game(null!));
    }

    [Fact]
    public void SpawnTile_Creates_NewTile_OnRandom_Position()
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

        Assert.Equal(3, game.Grid.GetCount());
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
    public void IdCounter_WorksAsExpected_WhenSpawn_NewTiles()
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
    public void IfThereIs_NoEmpty_Cells_SpawnWill_NotCrash_TheProgram()
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
    
}
