using Game2048.Maui.Achievements.Interfaces;
using Game2048.Maui.Enums;
using Game2048.Core.DTOs;
using Game2048.Core.Models;
using Game2048.Maui.Models;

namespace Game2048.Maui.Achievements.Checkers;

public class Play100GamesChecker : IGlobalAchievementChecker
{
    public AchievementType Type => AchievementType.Play100Games;

    public bool Check(PlayerProfile profile)
    {
        return profile.GlobalPlayerStatistics.TotalGamesPlayed >= 100;
    }
}

