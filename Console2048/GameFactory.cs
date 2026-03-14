namespace Game2048.Core;

public static class GameFactory
{
    public static Game CreateStandardGame(int width = 4, int height = 4, IRandomProvider? random = null)
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
}
