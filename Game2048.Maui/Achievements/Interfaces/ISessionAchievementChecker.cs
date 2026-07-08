using Game2048.Core.DTOs;
using Game2048.Core.Models;

namespace Game2048.Maui.Achievements.Interfaces;

public interface ISessionAchievementChecker : IAchievementChecker
{
    bool Check(StateSnapshot state, List<TileTransition> transitions);
}
