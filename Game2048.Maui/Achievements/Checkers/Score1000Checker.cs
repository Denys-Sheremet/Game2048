using Game2048.Maui.Achievements.Interfaces;
using Game2048.Maui.Enums;
using Game2048.Core.DTOs;
using Game2048.Core.Models;

namespace Game2048.Maui.Achievements.Checkers;

public class Score1000Checker : ISessionAchievementChecker
{
    public AchievementType Type => AchievementType.Score1000;

    public bool Check(StateSnapshot state, List<TileTransition> transitions)
    {
        return state.Score >= 1000;
    }
}

