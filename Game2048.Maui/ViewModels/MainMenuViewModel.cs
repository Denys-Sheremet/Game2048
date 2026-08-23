using CommunityToolkit.Mvvm.Input;
using Game2048.Core.Enums;
using Game2048.Maui.Enums;
using Game2048.Core.Models;
using Game2048.Maui.Interfaces;
using Game2048.Core.Interfaces;
using System.Windows.Input;

namespace Game2048.Maui.ViewModels;

public partial class MainMenuViewModel : BindableObject
{
    private readonly GameConfig _gameConfig;
    private readonly ISaveService _saveService;
    private readonly IProfileManager _profileManager;

    public IRelayCommand PlayCommand { get; }
    public IRelayCommand GoToGameModesCommand { get; }
    public IRelayCommand GoToAchievementsCommand { get; }
    public IRelayCommand GoToThemesCommand { get; }
    public IRelayCommand GoToProfileCommand { get; }
    public IRelayCommand GoToHowToCommand { get; }
    public IRelayCommand GoToSettingsCommand { get; }

    public event Action? PlayRequested;
    public event Action? GameModesRequested;
    public event Action? AchievementsRequested;
    public event Action? ThemesRequested;
    public event Action? ProfileRequested;
    public event Action? HowToPlayRequested;
    public event Action? SettingsRequested;

    public MainMenuViewModel(GameConfig gameConfig, ISaveService saveService, IProfileManager profileManager)
    {
        _gameConfig = gameConfig;
        _saveService = saveService;
        _profileManager = profileManager;

        PlayCommand = new RelayCommand(GoToGame);
        GoToGameModesCommand = new RelayCommand(GoToGameModes);
        GoToAchievementsCommand = new RelayCommand(GoToAchievements);
        GoToThemesCommand = new RelayCommand(GoToThemes);
        GoToProfileCommand = new RelayCommand(GoToProfile);
        GoToHowToCommand = new RelayCommand(GoToHowTo);
        GoToSettingsCommand = new RelayCommand(GoToSettings);
    }

    private void GoToGame() => PlayRequested?.Invoke();
    private void GoToGameModes() => GameModesRequested?.Invoke();
    private void GoToAchievements() => AchievementsRequested?.Invoke();
    private void GoToThemes() => ThemesRequested?.Invoke();
    private void GoToProfile() => ProfileRequested?.Invoke();
    private void GoToHowTo() => HowToPlayRequested?.Invoke();
    private void GoToSettings() => SettingsRequested?.Invoke();
}
