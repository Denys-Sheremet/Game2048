using Console2048;

Grid grid = new Grid(4, 4);
Game game = new Game(grid);
game.SpawnMultipleTiles(2);

Console.CursorVisible = false;

while (!game.IsGameOver)
{
    Console.SetCursorPosition(0, 0);

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("=== CONSOLE 2048 ===");
    Console.ResetColor();
    Console.WriteLine($"Score: {game.Grid.Score} | History: {game.HistoryCount}    ");
    Console.WriteLine("---------------------");

    DrawColoredGrid(game.Grid);

    Console.WriteLine("\n---------------------");
    Console.WriteLine("Arrows: Move | Z: Undo | Esc: Exit");

    var key = Console.ReadKey(true).Key;

    switch (key)
    {
        case ConsoleKey.UpArrow: game.Move(MoveDirection.Up); break;
        case ConsoleKey.DownArrow: game.Move(MoveDirection.Down); break;
        case ConsoleKey.LeftArrow: game.Move(MoveDirection.Left); break;
        case ConsoleKey.RightArrow: game.Move(MoveDirection.Right); break;
        case ConsoleKey.Z: game.Undo(); break;
        case ConsoleKey.Escape: return;
    }
}

Console.Clear();
DrawColoredGrid(game.Grid);
Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine("\n--- GAME OVER ---");
Console.ResetColor();
Console.WriteLine($"Final Score: {game.Grid.Score}");
Console.ReadLine();

void DrawColoredGrid(Grid grid)
{
    for (int y = 0; y < grid.Height; y++)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            Tile? tile = grid[x, y];
            int value = tile?.Value ?? 0;

            (ConsoleColor back, ConsoleColor front) colors = GetTileColors(value);
            Console.BackgroundColor = colors.back;
            Console.ForegroundColor = colors.front;

            string displayValue = value == 0 ? "." : value.ToString();
            Console.Write($" {displayValue.PadRight(4)}");

            Console.ResetColor();
            Console.Write(" ");
        }
        Console.WriteLine("\n");
    }
}

(ConsoleColor Background, ConsoleColor Foreground) GetTileColors(int value) => value switch
{
    0 => (ConsoleColor.DarkGray, ConsoleColor.Gray),
    2 => (ConsoleColor.Gray, ConsoleColor.Black),
    4 => (ConsoleColor.White, ConsoleColor.Black),
    8 => (ConsoleColor.Yellow, ConsoleColor.Black),
    16 => (ConsoleColor.DarkYellow, ConsoleColor.White),
    32 => (ConsoleColor.Red, ConsoleColor.White),
    64 => (ConsoleColor.DarkRed, ConsoleColor.White),
    128 => (ConsoleColor.Magenta, ConsoleColor.White),
    256 => (ConsoleColor.DarkMagenta, ConsoleColor.White),
    512 => (ConsoleColor.Cyan, ConsoleColor.Black),
    1024 => (ConsoleColor.Blue, ConsoleColor.White),
    2048 => (ConsoleColor.Green, ConsoleColor.White),
    _ => (ConsoleColor.DarkGreen, ConsoleColor.White)
};