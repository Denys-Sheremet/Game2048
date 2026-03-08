using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048;

public static class GameFactory
{
    public static Game CreateStandardGame(int width = 4, int height = 4)
    {
        Grid grid = new Grid(width, height);
        return new Game
            (
                grid,
                new TileSpawner(),
                new HistoryManager(),
                new TileRegistry(),
                new DefaultRandomProvider()
            );
    }
}
