using Console2048;
using Console2048.ConsoleUI;
using Spectre.Console;

Console2048.Grid grid = new Console2048.Grid(2, 2);
Game game = new Game(grid, new TileSpawner(), new HistoryManager(), new TileRegistry(), new DefaultRandomProvider());

Console.CursorVisible = false;

game.OnStateChanged += () => ConsoleUIRenderer.Render(game);

game.OnScoreGained += (points) =>
{
    ConsoleUIRenderer.SetLastBonus(points);
};

Console.CursorVisible = false;
game.SpawnMultipleTiles(2);

ConsoleUIRenderer.Render(game);

while (!game.IsGameOver && !game.IsVictory)
{
    var key = Console.ReadKey(true).Key;

    switch (key)
    {
        case ConsoleKey.UpArrow: game.Move(MoveDirection.Up); break;
        case ConsoleKey.DownArrow: game.Move(MoveDirection.Down); break;
        case ConsoleKey.LeftArrow: game.Move(MoveDirection.Left); break;
        case ConsoleKey.RightArrow: game.Move(MoveDirection.Right); break;
        case ConsoleKey.Z: game.Undo(); ConsoleUIRenderer.Render(game); break;
        case ConsoleKey.Escape: return;
    }
}

if (game.IsVictory)
{
    ConsoleUIRenderer.DrawVictory(game);
}
else if (game.IsGameOver)
{
    ConsoleUIRenderer.DrawGameOver(game);
}
Console.ReadLine();