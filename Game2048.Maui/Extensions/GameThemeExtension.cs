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

        GameTheme.TokyoTheme => 0,
        GameTheme.NatureTheme => 0,
        GameTheme.SolarTheme => 0,
        GameTheme.MatchaCreamTheme => 0,
        GameTheme.BWTheme => 0,

        GameTheme.NeonColorTheme => 0,
        GameTheme.RainbowTheme => 0,
        GameTheme.TerminalRetroTheme => 0,
        GameTheme.PinkyPinkTheme => 0,
        GameTheme.HazardTheme => 0,
        GameTheme.VaporWaveTheme => 0,
        GameTheme.SakuraTheme => 0,

        GameTheme.RoyalRedTheme => 0,
        GameTheme.RoyalBlueTheme => 0,
        GameTheme.AbyssTheme => 0,
        GameTheme.TheGreatCityTheme => 0,
        GameTheme.NebulaTheme => 0,
        GameTheme.BayHarbourTheme => 0,
        GameTheme.Cyberpunk2078Theme => 0,

        GameTheme.TetrisNumberlessTheme => 0,
        GameTheme.RarityNumberlessTheme => 0,
        GameTheme.EmeraldBloomNumberlessTheme => 0,
        GameTheme.MonochromeNumberlessTheme => 0,

        _ => 100
    };

    //public static int GetPrice(this GameTheme theme) => theme switch
    //{
    //    GameTheme.ClassicTheme => 0,
    //    GameTheme.DarkTheme => 0,

    //    GameTheme.TokyoTheme => 100,
    //    GameTheme.NatureTheme => 100,
    //    GameTheme.SolarTheme => 100,
    //    GameTheme.MatchaCreamTheme => 100,
    //    GameTheme.BWTheme => 100,

    //    GameTheme.NeonColorTheme => 150,
    //    GameTheme.RainbowTheme => 125,
    //    GameTheme.TerminalRetroTheme => 125,
    //    GameTheme.PinkyPinkTheme => 150,
    //    GameTheme.HazardTheme => 125,
    //    GameTheme.VaporWaveTheme => 125,
    //    GameTheme.SakuraTheme => 150,

    //    GameTheme.RoyalRedTheme => 200,
    //    GameTheme.RoyalBlueTheme => 200,
    //    GameTheme.AbyssTheme => 150,
    //    GameTheme.TheGreatCityTheme => 200,
    //    GameTheme.NebulaTheme => 150,
    //    GameTheme.BayHarbourTheme => 200,
    //    GameTheme.Cyberpunk2078Theme => 200,

    //    GameTheme.TetrisNumberlessTheme => 175,
    //    GameTheme.RarityNumberlessTheme => 175,
    //    GameTheme.EmeraldBloomNumberlessTheme => 175,
    //    GameTheme.MonochromeNumberlessTheme => 175,

    //    _ => 100
    //};
}
