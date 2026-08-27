using Game2048.Maui.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Game2048.Maui.ViewModels;

public partial class ProfilePageViewModel : BindableObject
{
    private readonly IProfileManager _profileManager;
    private readonly IThemesManager _themesManager;

    private double _totalGamesPlayed = 0;
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
    
    private double _totalGamesWon = 0;
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

    public string WinRatePercentage => $"{WinRateRatio * 100}%";
    private double _winRateRatio = 0;
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

    private string _achievementsCountText = "0 / 0";

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

    private double _achievementsProgressFraction = 0;
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

    private string _themesCountText = "0 / 0";

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

    private double _themesProgressFraction = 0;
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



    public ProfilePageViewModel(IProfileManager profileManager, IThemesManager themeManager)
    {
        _profileManager = profileManager;
        _themesManager = themeManager;
    }


}
