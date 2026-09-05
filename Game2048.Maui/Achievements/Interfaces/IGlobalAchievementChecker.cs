using Game2048.Core.Models;
using Game2048.Maui.Models;

namespace Game2048.Maui.Achievements.Interfaces;

public interface IGlobalAchievementChecker : IAchievementChecker
{
    bool Check(PlayerProfile profile);
}
