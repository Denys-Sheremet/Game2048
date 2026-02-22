using Console2048;

Grid grid = new Grid(4, 4);
Game game = new Game(grid);
game.SpawnMultipleTiles(2);

Console.CursorVisible = false;

while (!game.IsGameOver)
{
    Console.SetCursorPosition(0, 0);

    Console.WriteLine(game.Grid.ToString());
    Console.WriteLine($"Score: {game.Grid.Score}      ");
    var key = Console.ReadKey(true).Key;

    switch (key) {
        case ConsoleKey.UpArrow:    game.Move(MoveDirection.Up); break;
        case ConsoleKey.DownArrow:  game.Move(MoveDirection.Down); break;
        case ConsoleKey.LeftArrow:  game.Move(MoveDirection.Left); break;
        case ConsoleKey.RightArrow: game.Move(MoveDirection.Right); break;
        case ConsoleKey.Z: game.Undo(); break; 
        case ConsoleKey.Escape: return;

    }
}

Console.Clear();
Console.WriteLine(game.Grid.ToString());
Console.WriteLine("\n--- GAME OVER ---");
Console.WriteLine($"Score: {game.Grid.Score}");
Console.ReadLine();
