namespace Game2048.Core.Enums;

public enum GameModeType
{
    /// <summary> Classic 4x4 game no Undo </summary>
    Classic,
    /// <summary> Classic 4x4 game with Undo </summary>
    ClassicPlus,
    /// <summary> 3x3 game with Undo </summary>
    Compact,
    /// <summary> 5x5 game with Undo </summary>
    Extended,
    /// <summary> 5x5 game with unlimited history and Undo </summary>
    ChillZone
}
