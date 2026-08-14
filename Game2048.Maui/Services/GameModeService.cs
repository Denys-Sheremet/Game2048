using Game2048.Core.Enums;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Models;
using Game2048.Maui.Services;
using Game2048.Maui.Resources.Localization;

namespace Game2048.Maui.Services;

public class GameModeService : IGameModeService
{
    private readonly IThemesManager _themesManager;
    private readonly ISettingsManager _settingsManager;

    private IReadOnlyList<GameMode> _gameModes = new List<GameMode>()
    {
            new GameMode(GameModeType.Classic, AppResources.GameMode_Classic_title, AppResources.GameMode_Classic_desc, "classic_game_preview.gif", Colors.Transparent, false),
            new GameMode(GameModeType.ClassicPlus, AppResources.GameMode_ClassicPlus_title, AppResources.GameMode_ClassicPlus_desc, "classic_plus_game_preview.gif", Colors.Transparent, false),
            new GameMode(GameModeType.Compact, AppResources.GameMode_Compact_title, AppResources.GameMode_Compact_desc, "compact_game_preview.gif", Colors.Transparent, false),
            new GameMode(GameModeType.Extended, AppResources.GameMode_Extended_title, AppResources.GameMode_Extended_desc, "extended_game_preview.gif", Colors.Transparent, false),
            new GameMode(GameModeType.ChillZone, AppResources.GameMode_ChillZone_title, AppResources.GameMode_ChillZone_desc, "chill_zone_game_preview.gif", Colors.Transparent, false)
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
