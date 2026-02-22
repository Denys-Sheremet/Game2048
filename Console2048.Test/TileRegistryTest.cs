using Console2048;
using Xunit;

namespace Console2048.Test;

public class TileRegistryTest
{
    [Fact]
    public void RegistryRegisters_TilesWith_TheExact_PointerAnd_Id()
    {
        TileRegistry registry = new TileRegistry();
        Tile tile1 = new Tile(1, 0, 0, 0, 0, false, 2);
        Tile tile2 = new Tile(2, 1, 0, 0, 0, false, 2);

        registry.Register(tile1);

        Assert.Equal(1, registry.Count);
        Assert.Equal(tile1, registry[1]);
        Assert.Null(registry[2]);
    }

    [Fact]
    public void RegistryCan_RegisterMany_Tiles_AtOnce()
    {
        TileRegistry registry = new TileRegistry();
        Tile tile1 = new Tile(1, 0, 0, 0, 0, false, 2);
        Tile tile2 = new Tile(2, 1, 0, 0, 0, false, 2);

        registry.RegisterMany(new[] {tile1, tile2});

        Assert.Equal(2, registry.Count);
        Assert.Equal(tile1, registry[1]);
        Assert.Equal(tile2, registry[2]);
    }

    [Fact]
    public void RegistryUnregisters_TilesBy_ItsId()
    {
        TileRegistry registry = new TileRegistry();
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2);
        registry.Register(tile);

        registry.Unregister(1);

        Assert.Equal(0, registry.Count);
        Assert.Null(registry[1]);
    }

    [Fact]
    public void RegistryCan_UnregisterMany_Tiles_AtOnce()
    {
        TileRegistry registry = new TileRegistry();
        Tile tile1 = new Tile(1, 0, 0, 0, 0, false, 2);
        Tile tile2 = new Tile(2, 1, 0, 0, 0, false, 2);

        registry.Register(tile1);
        registry.Register(tile2);

        registry.UnregisterMany(new[] {tile1, tile2});

        Assert.Equal(0, registry.Count);
        Assert.Null(registry[1]);
        Assert.Null(registry[2]);
    }

    [Fact]
    public void Registry_ChangeTile_WithTheSame_IdInPlace()
    {
        TileRegistry registry = new TileRegistry();
        Tile tile = new Tile(1, 0, 0, 0, 0, false, 2);
        Tile sameIdTile = new Tile(1, 1, 1, 1, 1, false, 2);

        registry.Register(tile);
        registry.Register(sameIdTile);

        Assert.Equal(1, registry.Count);
        Assert.Equal(sameIdTile, registry[1]);
    }

    [Fact]
    public void Registry_WillNotCrush_IfUnregister_IdIt_DoesNotContain()
    {
        TileRegistry registry = new TileRegistry();

        Exception exception = Record.Exception(() => registry.Unregister(999));

        Assert.Null(exception);
    }

    [Fact]
    public void Registry_CanClear_AllTilesCorrectly()
    {
        TileRegistry registry = new TileRegistry();
        Tile tile1 = new Tile(1, 0, 0, 0, 0, false, 2);
        Tile tile2 = new Tile(2, 1, 0, 0, 0, false, 2);

        registry.Register(tile1);
        registry.Register(tile2);

        registry.Clear();

        Assert.Equal(0, registry.Count);
        Assert.Null(registry[1]);
        Assert.Null(registry[2]);
    }
}
