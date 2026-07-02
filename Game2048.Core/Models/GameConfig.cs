namespace Game2048.Core.Models;

public record GameConfig
{
    public int Rows { get; set; } = 4;
    public int Cols { get; set; } = 4;
    public GameModeType GameMode { get; set; } = GameModeType.Classic;

    public void SetConfig(GameModeType gameMode)
    { 
        GameMode = gameMode;
        switch (gameMode)
        {
            case GameModeType.Classic: Rows = 4; Cols = 4; break;
            case GameModeType.ClassicPlus: Rows = 4; Cols = 4; break;
            case GameModeType.Compact: Rows = 3; Cols = 3; break;
            case GameModeType.Extended: Rows = 5; Cols = 5; break;
            case GameModeType.ChillZone: Rows = 5; Cols = 5; break;
            default : throw new ArgumentException("GameModeType provided was not implemented yet");
        }
    }

    public GameConfig GetConfig() => this;
}
