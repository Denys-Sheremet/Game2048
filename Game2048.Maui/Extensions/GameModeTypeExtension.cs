using Game2048.Core.Enums;
using Game2048.Maui.Enums;
using Game2048.Maui.Resources.Localization;

namespace Game2048.Maui.Extensions;

public static class GameModeTypeExtension
{
    public static string GetTitle(this GameModeType type)
    {
        string key = $"GameMode_{type}_title";
        return AppResources.ResourceManager.GetString(key, AppResources.Culture) ?? type.ToString();
    }

    public static string GetDesc(this GameModeType type)
    {
        string key = $"GameMode_{type}_desc";
        return AppResources.ResourceManager.GetString(key, AppResources.Culture) ?? type.ToString();
    }
}
