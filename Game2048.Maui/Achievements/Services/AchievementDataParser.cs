using Game2048.Maui.Enums;
using Game2048.Maui.Resources.Localization;

namespace Game2048.Maui.Achievements.Services;

public static class AchievementDataParser
{
    //This service is needed to parse the achievement type from enum to strings 
    //for achievement's title, description and icon path

    public static string GetTitle(AchievementType achievementType)
    {
        string titleKey = $"Ach_{achievementType}_title";
        return AppResources.ResourceManager.GetString(titleKey) ?? achievementType.ToString();
    }

    public static string GetDesc(AchievementType achievementType)
    {
        string descKey = $"Ach_{achievementType}_desc";
        return AppResources.ResourceManager.GetString(descKey) ?? "NaN";
    }

    public static string GetImageName(AchievementType achievementType)
    {
        return $"ach_{achievementType.ToString().ToLower()}.png";
    }
}
