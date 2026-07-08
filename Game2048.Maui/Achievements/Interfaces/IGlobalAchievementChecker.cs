using Game2048.Maui.Models;

namespace Game2048.Maui.Achievements.Interfaces;

public interface IGlobalAchievementChecker : IAchievementChecker
{
    bool Check(PlayerStatistics stats);
}
