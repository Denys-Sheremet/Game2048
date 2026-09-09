using Game2048.Maui.Interfaces;
using Game2048.Maui.Extensions;
using System.Collections.ObjectModel;
using Game2048.Maui.ViewModels.Items;

namespace Game2048.Maui.ViewModels;

public partial class ProfilePageViewModel : BindableObject
{
    private readonly IProfileManager _profileManager;
    private readonly IThemesManager _themesManager;

    private string _playerName = String.Empty;
    private double _totalGamesPlayed = 0;
    private double _totalGamesWon = 0;
    private double _winRateRatio = 0;
    private string _achievementsCountText = "0 / 0";
    private double _achievementsProgressFraction = 0;
    private string _themesCountText = "0 / 0";
    private double _themesProgressFraction = 0;
    private double _overallHighScore = 0;
    private string _bestScoreModeName = String.Empty;
    private double _averageScore = 0;
    private double _totalCoinsEarned = 0;

    public ObservableCollection<ModeStatViewModel> ModeStats { get; } = new();
    public string WinRatePercentage => $"{(int)Math.Round(WinRateRatio * 100)}%";
    public bool IsModeStatsNotEmpty => ModeStats.Count > 0;
    public string PlayerName
    {
        get { return _playerName; }
        set
        {
            if (_playerName != value)
            {
                _playerName = value;
                OnPropertyChanged();
            }
        }
    }
    public double TotalGamesPlayed
    {
        get { return _totalGamesPlayed; }
        set 
        { 
            if (_totalGamesPlayed != value)
            {
                _totalGamesPlayed = value;
                OnPropertyChanged();
            }
        }
    }
    public double TotalGamesWon
    {
        get { return _totalGamesWon; }
        set
        {
            if (_totalGamesWon != value)
            {
                _totalGamesWon = value;
                OnPropertyChanged();
            }
        }
    }
    public double WinRateRatio
    {
        get { return _winRateRatio; }
        set
        {
            if (_winRateRatio != value)
            {
                _winRateRatio = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(WinRatePercentage));
            }
        }
    }
    public string AchievementsCountText
    {
        get => _achievementsCountText;
        set
        {
            if (_achievementsCountText != value)
            {
                _achievementsCountText = value;
                OnPropertyChanged();
            }
        }
    }
    public double AchievementsProgressFraction
    {
        get => _achievementsProgressFraction;
        set
        {
            if (_achievementsProgressFraction != value)
            {
                _achievementsProgressFraction = value;
                OnPropertyChanged();
            }
        }
    }
    public string ThemesCountText
    {
        get => _themesCountText;
        set
        {
            if (_themesCountText != value)
            {
                _themesCountText = value;
                OnPropertyChanged();
            }
        }
    }
    public double ThemesProgressFraction
    {
        get => _themesProgressFraction;
        set
        {
            if (_themesProgressFraction != value)
            {
                _themesProgressFraction = value;
                OnPropertyChanged();
            }
        }
    }
    public double OverallHighScore
    {
        get => _overallHighScore;
        set
        {
            if (_overallHighScore != value)
            {
                _overallHighScore = value;
                OnPropertyChanged();
            }
        }
    }
    public string BestScoreModeName
    {
        get => _bestScoreModeName;
        set
        {
            if (_bestScoreModeName != value)
            {
                _bestScoreModeName = value;
                OnPropertyChanged();
            }
        }
    }
    public double AverageScore
    {
        get => _averageScore;
        set
        {
            if (_averageScore != value)
            {
                _averageScore = value;
                OnPropertyChanged();
            }
        }
    }

    public double TotalCoinsEarned
    {
        get => _totalCoinsEarned;
        set
        {
            if (_totalCoinsEarned != value)
            {
                _totalCoinsEarned = value;
                OnPropertyChanged();
            }
        }
    }


    public ProfilePageViewModel(IProfileManager profileManager, IThemesManager themeManager)
    {
        _profileManager = profileManager;
        _themesManager = themeManager;

        LoadProfileData();
    }

    private void LoadProfileData()
    {
        var stats = _profileManager.GetGlobalStatistics();
        int totalAchievementsCount = AchievementTypeExtension.GetTotalAchievementsCount();
        int totalAchievementsUnlocked = _profileManager.GetUnlockedAchievements().Count;
        int totalThemesCount = GameThemeExtension.GetTotalThemesCount();
        int totalThemesUnlocked = _profileManager.GetUnlockedThemes().Count;
        var highestScore = _profileManager.GetOverallBestScore();

        PlayerName = _profileManager.GetPlayerName();
        TotalGamesPlayed = stats.TotalGamesPlayed;
        TotalGamesWon = stats.TotalGamesWon;

        WinRateRatio = TotalGamesPlayed > 0
            ? (double)TotalGamesWon / TotalGamesPlayed
            : 0;

        AchievementsCountText = $"{totalAchievementsUnlocked} / {totalAchievementsCount}";

        AchievementsProgressFraction = totalAchievementsCount > 0
            ? (double)totalAchievementsUnlocked / totalAchievementsCount
            : 0;

        ThemesCountText = $"{totalThemesUnlocked} / {totalThemesCount}";

        ThemesProgressFraction = totalThemesCount > 0
            ? (double)totalThemesUnlocked / totalThemesCount
            : 0;

        OverallHighScore = highestScore?.Score ?? 0;
        if (highestScore is null) BestScoreModeName = "None";
        else BestScoreModeName = highestScore.Value.Mode.GetTitle();
        AverageScore = _profileManager.GetAverageScore();
        TotalCoinsEarned = stats.TotalCoinsEarned;

        LoadModeStats();
    }

    private void LoadModeStats()
    {
        ModeStats.Clear();

        var sortedModes = _profileManager.GetBestScores().OrderByDescending(bs => bs.Value);

        int modePlace = 1;
        foreach (var mode in sortedModes) 
        {
            bool isTopMode = modePlace <= 3 && mode.Value > 0;
            ModeStats.Add(new ModeStatViewModel
            {
                ModeType = mode.Key,
                ModeName = mode.Key.GetTitle(),
                HighestScore = mode.Value,
                RatingIcon = isTopMode ? $"profile_top_{modePlace}.svg" : "profile_top_none.svg"
            });
            modePlace++;
        }
    }
}
