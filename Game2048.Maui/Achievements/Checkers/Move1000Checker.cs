using Game2048.Core.Models;
using Game2048.Maui.Achievements.Interfaces;
using Game2048.Maui.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game2048.Maui.Achievements.Checkers;

public class Move1000Checker : IGlobalAchievementChecker
{
    public AchievementType Type => AchievementType.Move1000;

    public bool Check(PlayerProfile profile)
    {
        return profile.GlobalPlayerStatistics.TotalMovesMade >= 1000;
    }
}
