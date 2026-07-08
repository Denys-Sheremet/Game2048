using Game2048.Maui.Enums;

namespace Game2048.Maui.Achievements.Interfaces;

public interface IAchievementChecker
{
    AchievementType Type { get; }
}
