using Game2048.Core.Enums;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Models;
using Game2048.Maui.Services;
using Game2048.Maui.Resources.Localization;
using Game2048.Maui.Extensions;

namespace Game2048.Maui.Services;

public class GameModeService : IGameModeService
{
    private readonly IThemesManager _themesManager;
    private readonly ISettingsManager _settingsManager;

    private readonly List<GameMode> _gameModes = new List<GameMode>()
    {
            new GameMode(GameModeType.Classic, GameModeType.Classic.GetTitle(), GameModeType.Classic.GetDesc(), "classic_game_preview.gif", Colors.Transparent, false),
            new GameMode(GameModeType.ClassicPlus, GameModeType.ClassicPlus.GetTitle(), GameModeType.ClassicPlus.GetDesc(), "classic_plus_game_preview.gif", Colors.Transparent, false),
            new GameMode(GameModeType.Compact, GameModeType.Compact.GetTitle(), GameModeType.Compact.GetDesc(), "compact_game_preview.gif", Colors.Transparent, false),
            new GameMode(GameModeType.Extended, GameModeType.Extended.GetTitle(), GameModeType.Extended.GetDesc(), "extended_game_preview.gif", Colors.Transparent, false),
            new GameMode(GameModeType.ChillZone, GameModeType.ChillZone.GetTitle(), GameModeType.ChillZone.GetDesc(), "chill_zone_game_preview.gif", Colors.Transparent, false)
    };

    public GameModeService(IThemesManager themesManager, ISettingsManager settingsManager)
    {
        _themesManager = themesManager;
        _settingsManager = settingsManager;
    }

    public IEnumerable<GameMode> GetAvailableGameModes()
    {
        var lastGameModeStr = _settingsManager.GetCurrentGameMode();
        GameModeType lastGameMode = Enum.TryParse<GameModeType>(lastGameModeStr, out var mode) ? mode : GameModeType.Classic;

        var gameModes = new List<GameMode>();

        for(int i = 0; i < _gameModes.Count; i++)
        {
            var gm = _gameModes[i];
            var isActive = (gm.ModeType == lastGameMode);
            string BGColorKey = $"Card{i + 1}BGColor";
            gm.CardColor = _themesManager.GetCurrentThemeBrush(BGColorKey);
            gm.IsActive = isActive;
            gameModes.Add(gm);
        }
        return gameModes;
    }
}
