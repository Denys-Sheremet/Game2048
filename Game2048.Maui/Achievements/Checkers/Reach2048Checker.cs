using Game2048.Maui.Achievements.Interfaces;
using Game2048.Maui.Enums;
using Game2048.Core.DTOs;
using Game2048.Core.Models;

namespace Game2048.Maui.Achievements.Checkers;

public class Reach2048Checker : ISessionAchievementChecker
{
    public AchievementType Type => AchievementType.Reach2048;

    public bool Check(StateSnapshot state, List<TileTransition> transitions)
    {
        return state.TileSnapshots.Any(t => t.Value == 2048);
    }
}
