namespace Game2048.Core;

public static class GameFactory
{
    public static Game CreateClassicGame(int width = 4, int height = 4, IRandomProvider? random = null)
    {
        Grid grid = new Grid(width, height);
        return new Game
            (
                grid,
                new TileSpawner(),
                new LimitedHistoryManager(5),
                new TileRegistry(),
                random ?? new DefaultRandomProvider()
            );
    }

    public static Game CreateClassicUnlimitedHistoryGame(int width = 4, int height = 4, IRandomProvider? random = null)
    {
        Grid grid = new Grid(width, height);
        return new Game
            (
                grid,
                new TileSpawner(),
                new HistoryManager(),
                new TileRegistry(),
                random ?? new DefaultRandomProvider()
            );
    }

    public static Game CreateClassicDelayedSpawnGame(int width = 3, int height = 3, int interval = 2, IRandomProvider? random = null, int initialSpawns = 2)
    {
        Grid grid = new Grid(width, height);
        return new Game
            (
                grid,
                new DelayedTileSpawner(interval, initialSpawns),
                new LimitedHistoryManager(5),
                new TileRegistry(),
                random ?? new DefaultRandomProvider()
            );
    }
}
