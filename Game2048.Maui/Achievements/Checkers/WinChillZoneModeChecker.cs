using Game2048.Maui.Achievements.Interfaces;
using Game2048.Maui.Enums;
using Game2048.Core.Enums;
using Game2048.Maui.Interfaces;

namespace Game2048.Maui.Achievements.Checkers;

public class WinChillZoneModeChecker : ISpecialAchievementChecker
{
    public AchievementType Type => AchievementType.WinChillZoneMode;
    public bool Check(IStatisticsManager statisticsManager)
    {
        return statisticsManager.HasWon && statisticsManager.CurrentGameMode == GameModeType.ChillZone;
    }
}
