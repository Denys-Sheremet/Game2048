using Game2048.Maui.Enums;

namespace Game2048.Maui.Extensions;

public static class AchievementTypeExtension
{
    public static int GetTotalAchievementsCount()
    {
        return Enum.GetValues<AchievementType>().Length;
    }
}
