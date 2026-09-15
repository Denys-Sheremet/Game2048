namespace Game2048.Core.Models;

public record GameConfig
{
    public int Rows { get; private set; } = 4;
    public int Cols { get; private set; } = 4;
    public GameModeType GameMode { get; private set; } = GameModeType.Classic;
    public int TargetValue { get; private set; } = 2048;

    public void SetConfig(GameModeType gameMode)
    { 
        GameMode = gameMode;
        switch (gameMode)
        {
            case GameModeType.Classic: Rows = 4; Cols = 4; TargetValue = 2048; break;
            case GameModeType.ClassicPlus: Rows = 4; Cols = 4; TargetValue = 2048; break;
            case GameModeType.Compact: Rows = 3; Cols = 3; TargetValue = 1024; break;
            case GameModeType.Extended: Rows = 5; Cols = 5; TargetValue = 4096; break;
            case GameModeType.ChillZone: Rows = 5; Cols = 5; TargetValue = 4096; break;
            default : throw new ArgumentException("GameModeType provided was not implemented yet");
        }
    }

    public GameConfig GetConfig() => this;
}
