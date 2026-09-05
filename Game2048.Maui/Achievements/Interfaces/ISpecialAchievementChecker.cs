using Game2048.Maui.Interfaces;

namespace Game2048.Maui.Achievements.Interfaces;

public interface ISpecialAchievementChecker : IAchievementChecker
{
    bool Check(IStatisticsManager statisticsManager);
}
