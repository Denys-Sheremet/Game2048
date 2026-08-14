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

    private void TryUnlock(AchievementType type)
    {
        if (_profileManager.UnlockAchievement(type))
        {
            AchievementUnlocked?.Invoke(type);
        }
    }

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
        if (_profileManager.CurrentProfile is null) return;

        var stats = _profileManager.CurrentProfile.GlobalPlayerStatistics;

        foreach (var ga in _globalAchievements)
        {
            if (_profileManager.IsAchievementUnlocked(ga.Type)) continue;
            if (ga.Check(stats))
            {
                TryUnlock(ga.Type);
            }
        }
    }

    public void CheckSessionAchievements(StateSnapshot afterState, List<TileTransition> transitions)
    {
        if (_profileManager.CurrentProfile is null) return;

        foreach (var sa in _sessionAchievements)
        {
            if (_profileManager.IsAchievementUnlocked(sa.Type)) continue;
            if (sa.Check(afterState, transitions))
            {
                TryUnlock(sa.Type);
            }
        }
    }

    public void CheckSpecialAchievements(IStatisticsManager statisticsManager)
    {
        if (_profileManager.CurrentProfile is null) return;

        var stats = statisticsManager.GetStatisticsManager();

        foreach (var sa in _specialAchievements)
        {
            if (_profileManager.IsAchievementUnlocked(sa.Type)) continue;
            if (sa.Check(stats.GameOver, stats.HasWon, stats.MovesMade, stats.UndosMade, stats.TilesMerged))
            {
                TryUnlock(sa.Type);
            }
        }
    }
}
