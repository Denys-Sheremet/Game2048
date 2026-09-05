using Game2048.Core.DTOs;
using Game2048.Core.Models;
using Game2048.Maui.Achievements.Interfaces;
using Game2048.Maui.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game2048.Maui.Achievements.Checkers;

public class Reach4096Checker : ISessionAchievementChecker
{
    public AchievementType Type => AchievementType.Reach4096;

    public bool Check(StateSnapshot state, List<TileTransition> transitions)
    {
        return state.TileSnapshots.Any(t => t.Value == 4096);
    }
}
