using Game2048.Maui.Achievements.Interfaces;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Enums;

namespace Game2048.Maui.Achievements.Checkers;

public class Why30Checker : ISpecialAchievementChecker
{
    public AchievementType Type => AchievementType.Why30;
    public bool Check(IStatisticsManager statisticsManager)
    {
        return statisticsManager.UselessUndoClicked >= 30;
    }
}
