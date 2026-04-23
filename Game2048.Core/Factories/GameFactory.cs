namespace Game2048.Core.Factories;

public static class GameFactory
{
    public static Game CreateGame(GameConfig config) => config.GameMode switch
    {
        GameModeType.Classic        => CreateClassicGame(config.Cols, config.Rows),
        GameModeType.ClassicPlus    => CreateClassicPlusGame(config.Cols, config.Rows),
        GameModeType.Compact        => CreateCompactGame(config.Cols, config.Rows),
        GameModeType.Extended       => CreateExtendedGame(config.Cols, config.Rows),
        GameModeType.ChillZone      => CreateChillZoneGame(config.Cols, config.Rows),
        _                           => throw new ArgumentException("No corresponding game mode implemented yet")
    };

    public static Game CreateClassicGame(int width = 4, int height = 4, IRandomProvider? random = null)
    {
        Grid grid = new Grid(width, height);
        return new Game
            (
                grid,
                new TileSpawner(),
                new DisabledHistoryManager(),
                new TileRegistry(),
                random ?? new DefaultRandomProvider()
            );
    }

    public static Game CreateClassicPlusGame(int width = 4, int height = 4, IRandomProvider? random = null)
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

    public static Game CreateCompactGame(int width = 3, int height = 3, int interval = 2, IRandomProvider? random = null, int initialSpawns = 2)
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

    public static Game CreateExtendedGame(int width = 5, int height = 5, IRandomProvider? random = null)
    {
        Grid grid = new Grid(width, height);
        return new Game
            (
                grid,
                new MultipleTileSpawner(2),
                new LimitedHistoryManager(5),
                new TileRegistry(),
                random ?? new DefaultRandomProvider()
            );
    }

    public static Game CreateChillZoneGame(int width = 5, int height = 5, IRandomProvider? random = null)
    {
        Grid grid = new Grid(width, height);
        return new Game
            (
                grid,
                new MultipleTileSpawner(2),
                new HistoryManager(),
                new TileRegistry(),
                random ?? new DefaultRandomProvider()
            );
    }
}
