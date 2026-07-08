using Game2048.Maui.Achievements.Interfaces;
using Game2048.Maui.Enums;
using Game2048.Core.DTOs;
using Game2048.Core.Models;

namespace Game2048.Maui.Achievements.Checkers;

public class Reach512Checker : ISessionAchievementChecker
{
    public AchievementType Type => AchievementType.Reach512;

    public bool Check(StateSnapshot state, List<TileTransition> transitions)
    {
        return state.TileSnapshots.Any(t => t.Value == 512);
    }
}
