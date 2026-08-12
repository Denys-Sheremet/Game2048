using Game2048.Core.DTOs;
using Game2048.Core.Models;
using Game2048.Maui.Enums;

namespace Game2048.Maui.Interfaces;

public interface IAchievementManager
{
    event Action<AchievementType>? AchievementUnlocked;
    void CheckGlobalAchievements();
    void CheckSessionAchievements(StateSnapshot afterState, List<TileTransition> transitions);
    void CheckSpecialAchievements(IStatisticsManager statisticsManager);
    void CheckAllAchievements(StateSnapshot afterState, List<TileTransition> transitions, IStatisticsManager statisticsManager);
}
