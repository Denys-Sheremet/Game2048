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

    public static List<string> GetCharacteristics(this GameModeType type)
    {
        List<string> keys = new()
        {
            $"GameMode_char_{type}_difficulty",
            $"GameMode_char_{type}_grid_size",
            $"GameMode_char_{type}_max_value",
            $"GameMode_char_{type}_extension_possible",
            $"GameMode_char_{type}_spawn_type",
            $"GameMode_char_{type}_undo_available"
        };

        List<string> characteristics = new();

        foreach (var key in keys)
        {
            string value = AppResources.ResourceManager.GetString(key, AppResources.Culture) ?? type.ToString();
            characteristics.Add(value);
        }

        if (characteristics.Count == 0)
        {
            characteristics.Add(type.ToString());
        }

        return characteristics;
    }
}
