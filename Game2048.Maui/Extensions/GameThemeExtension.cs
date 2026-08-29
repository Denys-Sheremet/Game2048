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
        GameTheme.NatureTheme => 0, //200
        GameTheme.NeonColorTheme => 0, //300
        GameTheme.RainbowTheme => 0, //300
        GameTheme.BWTheme => 0, //350
        GameTheme.PinkyPinkTheme => 0, //400
        GameTheme.RoyalRedTheme => 0, //500
        GameTheme.RoyalBlueTheme => 0, //500
        GameTheme.HazardTheme => 0, //350
        GameTheme.SolarTheme => 0, //200
        GameTheme.TokyoTheme => 0, //150
        _ => 100
    };
}
