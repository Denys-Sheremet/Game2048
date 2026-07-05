using Game2048.Core.DTOs;
using Game2048.Core.Models;
using Game2048.Maui.Enums;

namespace Game2048.Maui.Interfaces;

public interface IAchievementManager
{
    event Action<AchievementType>? OnAchievementUnlocked;
    Task AnalyzeTurnAsync(List<TileTransition> transitions, StateSnapshot afterState);
}
