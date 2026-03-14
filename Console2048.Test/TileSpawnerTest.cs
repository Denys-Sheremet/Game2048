using System;
using System.Collections.Generic;
using System.Linq;
using Game2048.Core.Mechanics;
using Game2048.Core.Models;
using Game2048.Core.Services;
using Game2048.Core.Interfaces;
using NSubstitute;


namespace Game2048.Core.Test;

public class TileSpawnerTest
{
    [Fact]
    public void Spawn_Returns_False_IfNo_EmptyCells()
    {
        Grid grid = new Grid(1, 1);
        grid[0, 0] = new Tile(999, 0, 0, 2);

        TileSpawner spawner = new TileSpawner();
        var mockRegistry = Substitute.For<ITileRegistry>();
        var mockRandom = Substitute.For<IRandomProvider>();

        bool hasSpawned = spawner.Spawn(grid, mockRegistry, 1, mockRandom);

        Assert.False(hasSpawned);
    }

    [Fact]
    public void Spawn_Uses_Correct_ValueProbabilities()
    {
        Grid grid = new Grid(1, 1);

        TileSpawner spawner = new TileSpawner();
        var mockRegistry = Substitute.For<ITileRegistry>();
        var mockRandom = Substitute.For<IRandomProvider>();
        mockRandom.Next(10).Returns(0, 1);

        spawner.Spawn(grid, mockRegistry, 1, mockRandom);

        Assert.Equal(4, grid[0, 0]!.Value);

        grid[0, 0] = null;

        spawner.Spawn(grid, mockRegistry, 2, mockRandom);

        Assert.Equal(2, grid[0, 0]!.Value);
    }

    [Fact]
    public void Spawn_Registers_Tile_InRegistry_WithCorrect_Id()
    {
        Grid grid = new Grid(1, 1);

        TileSpawner spawner = new TileSpawner();
        TileRegistry registry = new TileRegistry();

        Assert.Equal(0, registry.Count);

        var mockRandom = Substitute.For<IRandomProvider>();
        mockRandom.Next(10).Returns(0);

        spawner.Spawn(grid, registry, 999, mockRandom);

        Assert.Equal(1, registry.Count);
        Assert.Equal(999, registry[999]!.Id);
        Assert.Equal(4, registry[999]!.Value);
    }

    [Fact]
    public void Spawn_ChoosesCorrectEmptyCell_BasedOnRandomIndex()
    {
        Grid grid = new Grid(2, 2);
        grid[0, 0] = new Tile(1, 0, 0, 2);
        grid[1, 1] = new Tile(2, 1, 1, 2);

        var mockRandom = Substitute.For<IRandomProvider>();
        mockRandom.Next(2).Returns(0);
        mockRandom.Next(10).Returns(1);

        TileSpawner spawner = new TileSpawner();

        spawner.Spawn(grid, Substitute.For<ITileRegistry>(), 3, mockRandom);

        Tile? tile = grid[0, 1];

        Assert.NotNull(tile);
        Assert.Equal(3, tile!.Id);
        Assert.Equal(2, tile!.Value);
        Assert.Equal((0, 1), (tile!.PosX, tile!.PosY));
    }

    [Fact]
    public void TrySpawnAt_Returns_False_IfCellIsOccupied_AndDoes_NotChange_TheTile_InIt()
    {
        Grid grid = new Grid(2, 2);
        grid[0, 0] = new Tile(1, 0, 0, 2);
        TileSpawner spawner = new TileSpawner();

        var mockRegistry = Substitute.For<ITileRegistry>();

        bool hasSpawned = spawner.TrySpawnAt(grid, mockRegistry, 2, 0, 0, 4);

        Assert.False(hasSpawned);
        Assert.Equal(2, grid[0, 0]!.Value);
    }

    [Fact]
    public void TrySpawnAt_UsesProvidedValue_InsteadOfRandom()
    {
        Grid grid = new Grid(2, 2);
        TileSpawner spawner = new TileSpawner();

        var mockRegistry = Substitute.For<ITileRegistry>();

        spawner.TrySpawnAt(grid, mockRegistry, 1, 1, 1, 16);

        Assert.Equal(16, grid[1, 1]!.Value);
    }

    [Fact]
    public void Spawn_Throws_ArgumentNullException_If_RefTypeArguments_AreNull()
    {
        Grid grid = new Grid(2, 2);
        TileSpawner spawner = new TileSpawner();

        Assert.Throws<ArgumentNullException>
            (
                () => spawner.Spawn(null!, new TileRegistry(), 1, new DefaultRandomProvider())
            );
        Assert.Throws<ArgumentNullException>
            (
                () => spawner.Spawn(grid, null!, 1, new DefaultRandomProvider())
            );
        Assert.Throws<ArgumentNullException>
            (
                () => spawner.Spawn(grid, new TileRegistry(), 1, null!)
            );
    }

    [Fact]
    public void Spawn_Throws_ArgumentException_If_ProvidedId_IsNegative()
    {
        Grid grid = new Grid(2, 2);
        TileSpawner spawner = new TileSpawner();

        Assert.Throws<ArgumentException>
            (
                () => spawner.Spawn(grid, new TileRegistry(), -5, new DefaultRandomProvider())
            );
    }

    [Fact]
    public void TrySpawnAt_Throws_ArgumentNullException_If_RefType_Arguments_ExceptRandom_AreNull()
    {
        Grid grid = new Grid(2, 2);
        TileSpawner spawner = new TileSpawner();
        Assert.Throws<ArgumentNullException>
            (
                () => spawner.TrySpawnAt(null!, new TileRegistry(), 1, 0, 0, 2)
            );
        Assert.Throws<ArgumentNullException>
            (
                () => spawner.TrySpawnAt(grid, null!, 1, 0, 0, 2)
            );
    }

    [Fact]
    public void TrySpawnAt_Throws_ArgumentException_If_IdIs_Negative_OrIncorrectCoordinates()
    {
        Grid grid = new Grid(2, 2);
        TileSpawner spawner = new TileSpawner();
        Assert.Throws<ArgumentException>
            (
                () => spawner.TrySpawnAt(grid, new TileRegistry(), -1, 0, 0, 2)
            );
        Assert.Throws<ArgumentException>
            (
                () => spawner.TrySpawnAt(grid, new TileRegistry(), 1, 999, 0, 2)
            );
        Assert.Throws<ArgumentException>
            (
                () => spawner.TrySpawnAt(grid, new TileRegistry(), 1, 0, 999, 2)
            );
        Assert.Throws<ArgumentException>
            (
                () => spawner.TrySpawnAt(grid, new TileRegistry(), 1, -2, -5, 2)
            );
    }

    [Fact]
    public void TrySpawnAt_Throws_ArgumentException_If_Neither_Value_Nor_RandomProvider_Provided()
    {
        Grid grid = new Grid(2, 2);
        TileSpawner spawner = new TileSpawner();
        Assert.Throws<ArgumentNullException>
            (
                () => spawner.TrySpawnAt(grid, new TileRegistry(), 1, 0, 0)
            );
        Assert.Throws<ArgumentNullException>
            (
                () => spawner.TrySpawnAt(grid, new TileRegistry(), 1, 0, 0, value: null, random: null)
            );

    }
}
