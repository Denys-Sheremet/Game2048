using Game2048.Maui.Enums;
using Game2048.Maui.Resources.Localization;

namespace Game2048.Maui.Extensions;

public static class GameThemeExtension
{
    public static int GetTotalThemesCount()
    {
        return Enum.GetValues<GameTheme>().Length;
    }
    public static string GetTitle(this GameTheme theme)
    {
        string key =  $"{theme}_title";
        return AppResources.ResourceManager.GetString(key, AppResources.Culture) ?? theme.ToString();
    }

    public static string GetDesc(this GameTheme theme)
    {
        string key = $"{theme}_desc";
        return AppResources.ResourceManager.GetString(key, AppResources.Culture) ?? theme.ToString();
    }

    public static int GetPrice(this GameTheme theme) => theme switch
    {
        GameTheme.ClassicTheme => 0,
        GameTheme.DarkTheme => 0,

        GameTheme.TokyoTheme => 100,
        GameTheme.NatureTheme => 100,
        GameTheme.SolarTheme => 100,
        GameTheme.MatchaCreamTheme => 100,
        GameTheme.BWTheme => 100,
        GameTheme.TetrisNumberlessTheme => 100,

        GameTheme.NeonColorTheme => 150,
        GameTheme.RainbowTheme => 150,
        GameTheme.TerminalRetroTheme => 150,
        GameTheme.PinkyPinkTheme => 150,
        GameTheme.HazardTheme => 150,
        GameTheme.VaporWaveTheme => 150,
        GameTheme.SakuraTheme => 150,
        GameTheme.EmeraldBloomNumberlessTheme => 150,
        GameTheme.RarityNumberlessTheme => 150,
        GameTheme.MonochromeNumberlessTheme => 150,

        GameTheme.RoyalRedTheme => 200,
        GameTheme.RoyalBlueTheme => 200,
        GameTheme.AbyssTheme => 200,
        GameTheme.TheGreatCityTheme => 200,
        GameTheme.NebulaTheme => 200,
        GameTheme.BayHarbourTheme => 200,
        GameTheme.Cyberpunk2078Theme => 200,

        _ => 100
    };
}
