using Game2048.Core.Models;
using Game2048.Maui.Achievements.Interfaces;
using Game2048.Maui.Enums;
using Game2048.Maui.Extensions;

namespace Game2048.Maui.Achievements.Checkers;

public class AllAchievementsUnlockedChecker : IGlobalAchievementChecker
{
    public AchievementType Type => AchievementType.AllAchievementsUnlocked;
    public bool Check(PlayerProfile profile)
    {
        return (AchievementTypeExtension.GetTotalAchievementsCount() - 1)  <= profile.Achievements.Count;
    }
}
