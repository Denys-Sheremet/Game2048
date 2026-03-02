using Console2048;
using System.Linq;
using Xunit;
using NSubstitute;

namespace Console2048.Test;

public class GameTest
{
    [Fact]
    public void ConstructorInitialize_Correct_Data()
    {
        Grid grid = new Grid(4, 4);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        Assert.Equal(game.Grid, grid);
        Assert.True(game.HistoryIsEmpty());
        Assert.NotNull(game.TileRegistry);
        Assert.False(game.IsGameOver);
        Assert.Equal(1, game.GetNextTileId());
    }

    [Fact]
    public void ConstructorThrows_ArgumentNullException_IfGrid_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Game(null!, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider()));
    }

    [Fact]
    public void SpawnNewTile_SpawnsOne_TilePer_Call()
    {
        Grid grid = new Grid(4, 4);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        game.SpawnNewTile();

        Assert.Equal(1, game.Grid.Count);
    }

    [Fact]
    public void SpawnNewTile_GetsCoordinates_FromRandomProvider()
    {
        var mockRandomProvider = Substitute.For<IRandomProvider>();
        mockRandomProvider.Next(16).Returns(0);
        mockRandomProvider.Next(10).Returns(1);

        Grid grid = new Grid(4, 4);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), mockRandomProvider);

        game.SpawnNewTile();

        Tile? tile = game.Grid[0, 0];

        Assert.NotNull(tile);
        Assert.Equal(2, tile!.Value);
    }

    [Fact]
    public void SpawnNewTile_Spawns_TileWith_Value_4_In_10_Percents_Chance()
    {
        var mockRandomProvider = Substitute.For<IRandomProvider>();

        mockRandomProvider.Next(10).Returns(0, 1);

        Grid grid = new Grid(4, 4);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), mockRandomProvider);

        game.SpawnNewTile();
        Assert.Equal(4, game.Grid[0].Value);

        game.SpawnNewTile();
        Assert.Equal(2, game.Grid[1].Value);
    }

    [Fact]
    public void SpawnNewTile_Creates_NewTile_OnRandom_Position()
    {
        Grid grid = new Grid(4, 4);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());
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
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        game.SpawnMultipleTiles(3);

        Assert.Equal(3, game.Grid.Count);
        game.Grid.TryFindTile(1, out Tile? tile1);
        Assert.NotNull(tile1);
    }

    [Fact]
    public void WhenSpawn_NewTiles_ThereAre_NoCollisions_ByPosition()
    {
        Grid grid = new Grid(2, 2);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        game.SpawnMultipleTiles(2);
        game.Grid.TryFindTile(1, out Tile? tile1);
        game.Grid.TryFindTile(2, out Tile? tile2);

        Assert.False(tile1!.PosX == tile2!.PosX && tile1!.PosY == tile2!.PosY);
    }

    [Fact]
    public void SpawnNewTile_WhenGridIsFull_TriggersGameOverState()
    {
        Grid grid = new Grid(1, 1);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());
        game.SpawnNewTile(); //now grid is full

        game.SpawnNewTile();
        Assert.True(game.IsGameOver);
    }

    [Fact]
    public void TrySpawnNewTileAt_Returns_True_IfThe_Cell_WasEmpty_AndSpawned_Successfully()
    {
        Grid grid = new Grid(2, 2);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());
        Assert.True(game.TrySpawnNewTileAt(0, 0, 2));
    }

    [Fact]
    public void TrySpawnNewTileAt_Returns_False_IfThe_Cell_WasNotEmpty_AndSpawn_IsNotDone()
    {
        Grid grid = new Grid(2, 2);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());
        game.TrySpawnNewTileAt(0, 0, 2);
        Assert.False(game.TrySpawnNewTileAt(0, 0, 4));
    }

    [Fact]
    public void TrySpawnNewTileAt_UsesProvidedValue_ForNewTile()
    {
        int value = 16;
        Grid grid = new Grid(2, 2);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());
        game.TrySpawnNewTileAt(0, 0, value);

        Assert.Equal(value, game.Grid[0, 0]!.Value);
    }

    [Fact]
    public void TrySpawnNewTileAt_UsesStandard_Random_WhenValueIsNull()
    {
        Grid grid = new Grid(2, 2);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());
        game.TrySpawnNewTileAt(0, 0);

        int value = game.Grid[0, 0]!.Value;

        Assert.True(value == 2 || value == 4);
    }

    [Theory]
    [InlineData(-1, 0, 2)]
    [InlineData(0, -1, 2)]
    [InlineData(0, 99, 2)]
    [InlineData(99, 0, 2)]
    [InlineData(0, 0, 1)]
    [InlineData(0, 0, -5)]
    [InlineData(0, 0, int.MaxValue)]
    public void TrySpawnNewTileAt_Throws_ArgumentException_IfDimentions_AreIncorrect(int x, int y, int value)
    {
        Grid grid = new Grid(4, 4);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        Assert.Throws<ArgumentException>(() => game.TrySpawnNewTileAt(x, y, value));
    }

    [Fact]
    public void IdCounter_ReturnsNew_UniqueIds_AndIncrement_WhenSpawn_NewTiles()
    {
        Grid grid = new Grid(4, 4);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        int count = 5;
        game.SpawnMultipleTiles(count);

        for (int i = 1; i <= count; i++) 
        {
            game.Grid.TryFindTile(i, out Tile? tile);
            Assert.NotNull(tile);
        }
    }

    [Fact]
    public void IdWill_Continue_ToIncrement_EvenAfter_Undo_ForUnique_SpawnedTiles()
    {
        Grid grid = new Grid(4, 4);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());
        game.TrySpawnNewTileAt(0, 0, 2);
        game.TrySpawnNewTileAt(1, 0, 2);

        game.Move(MoveDirection.Right, false); //tiles merged and id 3 appeared
        game.Undo();
        game.TrySpawnNewTileAt(3, 3, 2); //must spawn a tile with next id - 4

        Assert.Equal(4, game.Grid[3, 3]!.Id);
    }

    [Fact]
    public void IfThereIs_NoEmpty_Cell_SpawnNewTile_Will_Not_Crash_TheProgram()
    {
        Grid grid = new Grid(1, 1);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        Exception exception = Record.Exception(() => game.SpawnMultipleTiles(5));

        Assert.Null(exception);
    }

    [Fact]
    public void MoveMethod_MovesTile_OnGridCorrectly()
    {
        Grid grid = new Grid(4, 4);
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2);
        grid[0, 0] = tile;
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

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

    [Fact]
    public void NewTile_WillNot_Spawn_If_WithAnimation_Flag_IsFalse()
    {
        Grid grid = new Grid(4, 4);
        grid[0, 0] = new Tile(1, 0, 0, 2);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        game.Move(MoveDirection.Right, withSpawn: false);
        game.Move(MoveDirection.Down, withSpawn: false);
        game.Move(MoveDirection.Left, withSpawn: false);
        game.Move(MoveDirection.Up, withSpawn: false);

        Assert.Equal(1, game.Grid.Count);
    }

    [Fact]
    public void MoveWith_NoActual_Movement_WillNot_ChangeThe_Grid()
    {
        Grid grid = new Grid(4, 4);
        Tile tile = new Tile(1, 0, 0, 2);
        grid[0, 0] = tile;
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        game.Move(MoveDirection.Left, false);

        Assert.Equal(tile, game.Grid[0, 0]);
        Assert.Equal(1, game.Grid.Count);
    }

    [Fact]
    public void OneTile_CanBe_MergedOnce_PerMove()
    {
        Grid grid = new Grid(4, 4);
        grid[0, 0] = new Tile(1, 0, 0, 2);
        grid[1, 0] = new Tile(2, 1, 0, 2);
        grid[2, 0] = new Tile(3, 2, 0, 2);
        grid[3, 0] = new Tile(4, 3, 0, 2);

        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        //should be 0044 not 0008 in one move
        game.Move(MoveDirection.Right, false);

        Assert.Equal(2, game.Grid.Count);
        Assert.Equal(4, game.Grid[0].Value);
        Assert.True(game.Grid[0].IsMerged);
        Assert.Equal(4, game.Grid[1].Value);
        Assert.True(game.Grid[1].IsMerged);
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

        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

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

        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

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
    public void Every_Move_WithMovement_HistoryCount_Increases_ByOne()
    {
        Grid grid = new Grid(4, 4);
        grid[0, 0] = new Tile(1, 0, 0, 2);

        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        game.Move(MoveDirection.Right, false);

        Assert.Equal(1, game.HistoryCount);
    }

    [Fact]
    public void IfTiles_DidNot_MoveAfter_MoveMethod_HistoryCount_Will_NotIncrease()
    {
        Grid grid = new Grid(4, 4);
        grid[0, 0] = new Tile(1, 0, 0, 2);

        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        game.Move(MoveDirection.Left, false);

        Assert.Equal(0, game.HistoryCount);
    }

    [Fact]
    public void Undo_Will_RestoreThe_GridFrom_ThePrevious_State_AndDecrease_HistoryCount()
    {
        Grid grid = new Grid(4, 4);
        Tile tile = new Tile(1, 0, 0, 2);
        grid[0, 0] = tile;

        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        game.Move(MoveDirection.Right, false);
        game.Undo();

        Assert.Equal(0, game.HistoryCount);
        Assert.Equal(0, game.Grid[0, 0]!.PosX);
        Assert.Equal(0, game.Grid[0, 0]!.PosY);
        Assert.Equal(1, game.Grid.Count);
    }

    [Fact]
    public void Undo_WillNot_ChangeThe_Grid_IfHistory_IsEmpty()
    {
        Grid grid = new Grid(4, 4);
        Tile tile = new Tile(1, 0, 0, 2);
        grid[0, 0] = tile;

        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        game.Undo();

        Assert.Equal(0, game.HistoryCount);
        Assert.Equal(tile, game.Grid[0, 0]);
        Assert.Equal(1, game.Grid.Count);
    }

    [Fact]
    public void History_WillNot_ChangeIts_States_InMultiple_Moves()
    {
        Grid grid = new Grid(4, 4);
        Tile tile = new Tile(1, 0, 0, 2);
        grid[0, 0] = tile;

        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        game.Move(MoveDirection.Right, false);
        game.Move(MoveDirection.Down, false);
        game.Move(MoveDirection.Left, false);

        game.Undo();
        game.Undo();
        game.Undo();

        Assert.Equal(0, game.HistoryCount);
        Assert.Equal(0, game.Grid[0, 0]!.PosX);
        Assert.Equal(0, game.Grid[0, 0]!.PosY);
        Assert.Equal(1, game.Grid.Count);
    }

    [Fact]
    public void Undo_Will_Restore_Previous_Score()
    {
        Grid grid = new Grid(4, 4);
        Tile tile1 = new Tile(1, 0, 0, 8);
        Tile tile2 = new Tile(2, 1, 0, 8);
        Tile tile3 = new Tile(3, 1, 0, 8);
        Tile tile4 = new Tile(4, 1, 0, 8);
        grid[0, 0] = tile1;
        grid[1, 0] = tile2;
        grid[2, 0] = tile3;
        grid[3, 0] = tile4;

        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());
        game.Move(MoveDirection.Right, false);

        int rememberedScore = game.Grid.Score;

        game.Move(MoveDirection.Right, false);
        game.Undo();

        Assert.Equal(rememberedScore, game.Grid.Score);
    }

    [Fact]
    public void Undo_Will_RegisterAll_RestoredTiles_ToRegistry()
    {
        Grid grid = new Grid(4, 4);
        Tile tile1 = new Tile(1, 0, 0, 8);
        Tile tile2 = new Tile(2, 1, 0, 8);
        Tile tile3 = new Tile(3, 1, 0, 8);
        Tile tile4 = new Tile(4, 1, 0, 8);
        grid[0, 0] = tile1;
        grid[1, 0] = tile2;
        grid[2, 0] = tile3;
        grid[3, 0] = tile4;

        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        game.Move(MoveDirection.Right, false);
        game.Move(MoveDirection.Right, false);
        game.Move(MoveDirection.Down, false);
        game.Move(MoveDirection.Left, false);

        while (game.HistoryCount > 0)
        {
            game.Undo();

            for (int i = 0; i < game.Grid.Count; i++)
            {
                Assert.NotNull
                    (
                        game.TileRegistry[game.Grid[i].Id]
                    );
                Assert.Equal(game.Grid.Count, game.TileRegistry.Count);
            }
        }
    }

    [Fact]
    public void Undo_SetsPrevious_Coordinates_OfParents_AsPosition_OfMerged_Tile()
    {
        Grid grid = new Grid(4, 4);
        grid[0, 0] = new Tile(1, 0, 0, 2);
        grid[1, 0] = new Tile(2, 1, 0, 2);

        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        game.Move(MoveDirection.Right, false);
        game.Undo();

        Assert.Equal((3, 0), (game.Grid[0, 0]!.PreviousX, game.Grid[0, 0]!.PreviousY));
        Assert.Equal((3, 0), (game.Grid[1, 0]!.PreviousX, game.Grid[1, 0]!.PreviousY));
    }

    [Fact]
    public void IsGameOver_IsTrueIf_GridIsFull_AndNoPossible_Moves()
    {
        Grid grid = new Grid(2, 2);
        grid[0, 0] = new Tile(1, 0, 0, 2);
        grid[1, 0] = new Tile(2, 0, 0, 4);
        grid[0, 1] = new Tile(3, 0, 0, 8);
        grid[1, 1] = new Tile(4, 0, 0, 16);

        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        game.CheckForGameOver();

        Assert.True(game.IsGameOver);
    }

    [Fact]
    public void IsGameOver_IsFalse_IfGridIs_Full_But_ThereAre_PossibleMoves()
    {
        Grid grid = new Grid(2, 2);
        grid[0, 0] = new Tile(1, 0, 0, 2);
        grid[1, 0] = new Tile(2, 0, 0, 2);
        grid[0, 1] = new Tile(3, 0, 0, 2);
        grid[1, 1] = new Tile(4, 0, 0, 2);

        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        game.CheckForGameOver();

        Assert.False(game.IsGameOver);
    }

    [Fact]
    public void IsGameOver_IsFalse_IfGridIs_Full_But_ThereAre_Possible_Horisontal_Moves()
    {
        Grid grid = new Grid(2, 1);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());
        game.TrySpawnNewTileAt(0, 0, 2);
        game.TrySpawnNewTileAt(1, 0, 2);

        game.CheckForGameOver();

        Assert.False(game.IsGameOver);
    }

    [Fact]
    public void IsGameOver_IsFalse_IfGridIs_Full_But_ThereAre_Possible_Vertical_Moves()
    {
        Grid grid = new Grid(1, 2);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());
        game.TrySpawnNewTileAt(0, 0, 2);
        game.TrySpawnNewTileAt(0, 1, 2);

        game.CheckForGameOver();

        Assert.False(game.IsGameOver);
    }


    [Fact]
    public void OnGameOver_Method_Sets_IsGameOver_ToTrue()
    {
        Grid grid = new Grid(1, 1);
        grid[0, 0] = new Tile(1, 0, 0, 2);
        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        game.CheckForGameOver(); //private OnGameOver() is being called

        Assert.True(game.IsGameOver);
    }

    [Fact]
    public void CheckForGameOver_WillNot_Crash_TheProgram_OnSingleDimention_Grid()
    {
        Grid grid = new Grid(1, 1);
        grid[0, 0] = new Tile(1, 0, 0, 2);

        Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

        game.CheckForGameOver();

        Assert.True(game.IsGameOver);
    }
}
