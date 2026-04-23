namespace Game2048.Core.Models;

public record GameConfig
{
    public int Rows { get; set; } = 4;
    public int Cols { get; set; } = 4;
    public GameModeType GameMode { get; set; } = GameModeType.Classic;

    public void SetConfig(int rows, int cols, GameModeType gameMode)
    {
        if (rows <= 0 || cols <= 0) throw new ArgumentException("Rows and Cols should be > 0");
        Rows = rows;
        Cols = cols;
        GameMode = gameMode;
    }
}
