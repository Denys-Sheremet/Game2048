using Game2048.Core.DTOs;
using Game2048.Core.Enums;
using Game2048.Core.Models;
using Game2048.Maui.Achievements.Interfaces;
using Game2048.Maui.Enums;
using Game2048.Maui.Interfaces;
using System.Collections.Generic;

namespace Game2048.Maui.Services;

public class AchievementManager : IAchievementManager
{
    private readonly IProfileManager _profileManager;
    private readonly List<IGlobalAchievementChecker> _globalAchievements;
    private readonly List<ISessionAchievementChecker> _sessionAchievements;
    private readonly List<ISpecialAchievementChecker> _specialAchievements;

    public event Action<AchievementType>? AchievementUnlocked;

    public AchievementManager(IProfileManager profileManager, 
        IEnumerable<IGlobalAchievementChecker> globalAchievements,
        IEnumerable<ISessionAchievementChecker> sessionAchievements,
        IEnumerable<ISpecialAchievementChecker> specialAchievements)
    {
        _profileManager = profileManager;
        _globalAchievements = globalAchievements.ToList();
        _sessionAchievements = sessionAchievements.ToList();
        _specialAchievements = specialAchievements.ToList();
    }

    public void CheckAllAchievements(StateSnapshot afterState, List<TileTransition> transitions, IStatisticsManager statisticsManager)
    {
        CheckGlobalAchievements();
        CheckSessionAchievements(afterState, transitions);
        CheckSpecialAchievements(statisticsManager);
    }

    public void CheckGlobalAchievements()
    {
        ArgumentNullException.ThrowIfNull(_profileManager.CurrentProfile);

        var stats = _profileManager.CurrentProfile.GlobalPlayerStatistics;
        var unlockedAchievements = _profileManager.GetUnlockedAchievements();

        foreach (var ga in _globalAchievements)
        {
            if (unlockedAchievements.Contains(ga.Type)) continue;
            if (ga.Check(stats))
            {
                AchievementUnlocked?.Invoke(ga.Type);
            }
        }
    }

    public void CheckSessionAchievements(StateSnapshot afterState, List<TileTransition> transitions)
    {
        ArgumentNullException.ThrowIfNull(_profileManager.CurrentProfile);

        var unlockedAchievements = _profileManager.GetUnlockedAchievements();

        foreach (var sa in _sessionAchievements)
        {
            if (unlockedAchievements.Contains(sa.Type)) continue;
            if (sa.Check(afterState, transitions))
            {
                AchievementUnlocked?.Invoke(sa.Type);
            }
        }
    }

    public void CheckSpecialAchievements(IStatisticsManager statisticsManager)
    {
        ArgumentNullException.ThrowIfNull(_profileManager.CurrentProfile);

        var stats = statisticsManager.GetStatisticsManager();
        var unlockedAchievements = _profileManager.GetUnlockedAchievements();

        foreach (var sa in _specialAchievements)
        {
            if (unlockedAchievements.Contains(sa.Type)) continue;
            if (sa.Check(stats.GameOver, stats.HasWon, stats.MovesMade, stats.UndosMade, stats.TilesMerged))
            {
                AchievementUnlocked?.Invoke(sa.Type);
            }
        }
    }
}
