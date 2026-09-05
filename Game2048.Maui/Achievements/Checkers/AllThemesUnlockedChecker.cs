using Game2048.Core.Models;
using Game2048.Maui.Achievements.Interfaces;
using Game2048.Maui.Enums;
using Game2048.Maui.Extensions;

namespace Game2048.Maui.Achievements.Checkers;

public class AllThemesUnlockedChecker : IGlobalAchievementChecker
{
    public AchievementType Type => AchievementType.AllThemesUnlocked;
    public bool Check(PlayerProfile profile)
    {
        return GameThemeExtension.GetTotalThemesCount() == profile.UnlockedThemes.Count;
    }
}
