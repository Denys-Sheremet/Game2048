using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace Console2048.ConsoleUI;

internal static class ConsoleUIRenderer
{
    private static int _lastBonus = 0;

    public static void SetLastBonus(int points)
    {
        _lastBonus = points;
    }

    public static void Render(Game game)
    {
        AnsiConsole.Clear();

        string bonusText = _lastBonus > 0 ? $" [bold green]+{_lastBonus}[/]" : "   ";

        //Basically the "logo" 2048 in big letters
        AnsiConsole.Write(
            new FigletText("2048 GAME")
                .Centered()
                .Color(Color.Orange3));

        Panel infoPanel = new Panel(Align.Center(
            new Markup($"[bold white]MOVES:[/] [blue]{game.HistoryCount}[/]   |   [bold white]SCORE:[/] [yellow]{game.Grid.Score}[/]{bonusText}")))
            .BorderColor(Color.Grey15);

        AnsiConsole.Write(infoPanel);

        //Table for field renderring
        Table table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Black)
            .Centered()
            .HideHeaders();

        for (int i = 0; i < game.Grid.Width; i++)
        {
            table.AddColumn(new TableColumn("").Width(6).Centered());
        }

        for (int y = 0; y < game.Grid.Height; y++)
        {
            List<Panel> rowTiles = new List<Panel>();
            for (int x = 0; x < game.Grid.Width; x++)
            {
                Tile? tile = game.Grid[x, y];
                rowTiles.Add(CreateTilePanel(tile?.Value ?? 0));
            }
            table.AddRow(rowTiles.ToArray());
        }

        AnsiConsole.Write(table);


        AnsiConsole.Write(new Markup("[grey]WASD:[/] Move | [grey]Z:[/] Undo | [grey]ESC:[/] Exit").Centered());
        AnsiConsole.WriteLine();

        _lastBonus = 0;
    }

    private static Panel CreateTilePanel(int value)
    {
        string displayText = value == 0 ? " " : value.ToString();
        Color tileColor = GetTileColor(value);
        Color textColor = value <= 16 ? Color.Black : Color.White;

        Text content = new Text(displayText, new Style(textColor, tileColor)).Centered();
        return new Panel(content)
            .Expand()
            .Padding(0, 1, 0, 1)
            .Border(BoxBorder.Ascii);
    }

    private static Color GetTileColor(int value) => value switch
    {
        2 => Color.FromHex("#eee4da"),
        4 => Color.FromHex("#ede0c8"),
        8 => Color.FromHex("#b3ff00"),
        16 => Color.FromHex("#ff4400"),
        32 => Color.FromHex("#2f00ff"),
        64 => Color.FromHex("#f53b63"),
        128 => Color.FromHex("#ff00e6"),
        256 => Color.FromHex("#940085"),
        512 => Color.FromHex("#e6b400"),
        1024 => Color.FromHex("#26ff00"),
        2048 => Color.FromHex("#00eeff"),
        _ => Color.FromHex("#525252")
    };

    public static void DrawGameOver(Game game)
    {
        AnsiConsole.Clear();

        var gameOverRule = new Rule("[bold red]GAME OVER[/]")
            .RuleStyle("red")
            .Centered();

        AnsiConsole.Write(gameOverRule);

        AnsiConsole.Write(
            new FigletText("DEFEAT")
                .Centered()
                .Color(Color.Red));

        var stats = new Panel(Align.Center(
            new Markup($"[bold white]FINAL SCORE:[/] [yellow]{game.Grid.Score}[/]\n" +
                       $"[bold white]TOTAL MOVES:[/] [blue]{game.HistoryCount}[/]")))
            .Header("[bold red] Defeat Summary [/]")
            .BorderColor(Color.Red)
            .Padding(2, 1, 2, 1);

        AnsiConsole.Write(new Padder(stats).Padding(0, 1, 0, 1));

        AnsiConsole.Write(new Markup("[bold white]No more possible moves left![/]").Centered());
        AnsiConsole.WriteLine();
    }

    public static void DrawVictory(Game game)
    {
        AnsiConsole.Clear();

        var victoryRule = new Rule("[bold yellow]CONGRATULATIONS[/]")
            .RuleStyle("gold1")
            .Centered();

        AnsiConsole.Write(victoryRule);

        AnsiConsole.Write(
            new FigletText("YOU WIN!")
                .Centered()
                .Color(Color.Cyan1));

        var stats = new Panel(Align.Center(
            new Markup($"[bold white]FINAL SCORE:[/] [yellow]{game.Grid.Score}[/]\n" +
                       $"[bold white]TOTAL MOVES:[/] [blue]{game.HistoryCount}[/]")))
            .Header("[bold green] Victory Summary [/]")
            .BorderColor(Color.Gold1)
            .Padding(2, 1, 2, 1);

        AnsiConsole.Write(new Padder(stats).Padding(0, 1, 0, 1));

        AnsiConsole.Write(new Markup("[bold white]You have reached the [cyan]2048[/] tile![/]").Centered());
        AnsiConsole.WriteLine();
    }
}
