using Game2048.Core.Enums;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Models;

namespace Game2048.Maui.Services;

public class GameModeService : IGameModeService
{
    public IEnumerable<GameMode> GetAvailableGameModes()
    {
        //can be rewritten to read from a file etc.
        //now will be hardcoded
        return new List<GameMode>
        {
            new GameMode(GameModeType.Classic, "Classic", "4x4", "classic_game_preview.gif", Color.FromArgb("#5A3211"), true),
            new GameMode(GameModeType.ClassicPlus, "ClassicPlus", "4x4", "classic_plus_game_preview.gif", Color.FromArgb("#964405"), false),
            new GameMode(GameModeType.Compact, "Compact", "3x3", "compact_game_preview.gif", Color.FromArgb("#D68631"), false),
            new GameMode(GameModeType.Extended, "Extended", "5x5", "extended_game_preview.gif", Color.FromArgb("#3D717E"), false),
            new GameMode(GameModeType.ChillZone, "ChillZone", "5x5", "chill_zone_game_preview.gif", Color.FromArgb("#10475E"), false)
        };
    }
}
