using Game2048.Core.DTOs;
using Game2048.Core.Enums;
using Game2048.Core.Models;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Enums;

namespace Game2048.Maui.Services;

public class AchievementManager : IAchievementManager
{
    private readonly IProfileManager _profileManager;

    public event Action<AchievementType>? OnAchievementUnlocked;

    //other useful properties will be added to count moves etc. (session stats)

    public AchievementManager(IProfileManager profileManager)
    {
        _profileManager = profileManager;
    }

    public async Task AnalyzeTurnAsync(List<TileTransition> transitions, StateSnapshot afterState)
    {
        // Analyze the turn and check for achievements
        //await CheckForBaseAchievementsAsync(transitions, afterState);
        //await CheckForTileAchievementsAsync(afterState);
        //await CheckForScoreAchievementsAsync(afterState);
    }
}
