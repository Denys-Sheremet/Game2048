using Game2048.Maui.Achievements.Interfaces;
using Game2048.Maui.Enums;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Models;

namespace Game2048.Maui.Achievements.Checkers;

public class NoUndoVictoryChecker : ISpecialAchievementChecker
{
    public AchievementType Type => AchievementType.NoUndoVictory;

    public bool Check(IStatisticsManager statisticsManager)
    {
        return statisticsManager.HasWon && statisticsManager.UndosMade == 0;
    }
}
