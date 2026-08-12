using Game2048.Maui.Interfaces;
using Game2048.Maui.Enums;
using Game2048.Maui.Achievements.Interfaces;

namespace Game2048.Maui.Services;

public partial class RewardManager : IRewardManager, IDisposable
{
    private readonly IProfileManager _profileManager;
    private readonly IAchievementManager _achievementManager;

    private int _lastWonCount;
    private int _lastLostCount;
    private int _lastThemesCount;
    private bool _isInitialized = false;

    public RewardManager(IProfileManager profileManager, IAchievementManager achievementManager)
    {
        _profileManager = profileManager;
        _achievementManager = achievementManager;

        _profileManager.ThemeUnlocked += OnThemeUnlocked;
        _profileManager.StatisticsUpdated += OnStatisticsUpdated;
        _achievementManager.AchievementUnlocked += OnAchievementUnlocked;

        InitializeLastStats();
    }

    private void InitializeLastStats()
    {
        if (_isInitialized || _profileManager.CurrentProfile is null) return;

        var stats = _profileManager.GetGlobalStatistics();

        int won = stats.TotalGamesWon;
        int lost = stats.TotalGamesPlayed - won;
        int themes = _profileManager.GetUnlockedThemes().Count;

        _lastWonCount = (won / 5) * 5;
        _lastLostCount = (lost / 5) * 5;
        _lastThemesCount = (themes / 5) * 5;

        _isInitialized = true;
    }

    public void Dispose()
    {
        _profileManager.ThemeUnlocked -= OnThemeUnlocked;
        _profileManager.StatisticsUpdated -= OnStatisticsUpdated;
        _achievementManager.AchievementUnlocked -= OnAchievementUnlocked;
    }

    private void OnThemeUnlocked(GameTheme theme)
    {
        InitializeLastStats();

        int themesCount = _profileManager.GetUnlockedThemes().Count;

        if (themesCount > _lastThemesCount)
        {
            if (themesCount % 5 == 0)
            {
                SmallReward();
            }
            _lastThemesCount = themesCount;
        }
    }

    private void OnStatisticsUpdated()
    {
        InitializeLastStats();

        var stats = _profileManager.GetGlobalStatistics();
        int gamesWon = stats.TotalGamesWon;
        int gamesLost = stats.TotalGamesPlayed - gamesWon;

        if (gamesWon > _lastWonCount)
        {
            if (gamesWon % 5 == 0)
            {
                MediumReward();
            }
            _lastWonCount = gamesWon;
        }

        if (gamesLost > _lastLostCount)
        {
            if (gamesLost % 5 == 0)
            {
                SmallReward();
            }
            _lastLostCount = gamesLost;
        }
    }

    private void OnAchievementUnlocked(AchievementType achievementType)
    {
        LargeReward();
    }

    private void SmallReward() => _profileManager.EarnCoins(25);
    private void MediumReward() => _profileManager.EarnCoins(50);
    private void LargeReward() => _profileManager.EarnCoins(100);
}
