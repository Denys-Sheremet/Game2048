using Game2048.Core.Models;
using Game2048.Maui.Achievements.Interfaces;
using Game2048.Maui.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game2048.Maui.Achievements.Checkers;
public class Earn1000CoinsChecker : IGlobalAchievementChecker
{
    public AchievementType Type => AchievementType.Earn1000Coins;

    public bool Check(PlayerProfile profile)
    {
        return profile.GlobalPlayerStatistics.TotalCoinsEarned >= 1000;
    }
}