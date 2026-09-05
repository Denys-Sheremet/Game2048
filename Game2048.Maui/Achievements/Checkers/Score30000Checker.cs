using Game2048.Core.DTOs;
using Game2048.Core.Models;
using Game2048.Maui.Achievements.Interfaces;
using Game2048.Maui.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game2048.Maui.Achievements.Checkers;

public class Score30000Checker : ISessionAchievementChecker
{
    public AchievementType Type => AchievementType.Score30000;

    public bool Check(StateSnapshot state, List<TileTransition> transitions)
    {
        return state.Score >= 30000;
    }
}
