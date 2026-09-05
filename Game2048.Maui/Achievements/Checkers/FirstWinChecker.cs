using Game2048.Core.Models;
using Game2048.Maui.Achievements.Interfaces;
using Game2048.Maui.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game2048.Maui.Achievements.Checkers;

public class FirstWinChecker : IGlobalAchievementChecker
{
    public AchievementType Type => AchievementType.FirstWin;

    public bool Check(PlayerProfile profile)
    {
        return profile.GlobalPlayerStatistics.TotalGamesWon >= 1;
    }
}
