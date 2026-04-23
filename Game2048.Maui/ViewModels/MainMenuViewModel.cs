using CommunityToolkit.Mvvm.Input;
using Game2048.Core.Models;
using System.Windows.Input;

namespace Game2048.Maui.ViewModels;

public partial class MainMenuViewModel : BindableObject
{
    public IAsyncRelayCommand StartClassicGameCommand { get; }
    public IAsyncRelayCommand GoToAchievementsCommand { get; }
    public IAsyncRelayCommand GoToGameModesCommand { get; }
    public IAsyncRelayCommand GoToSettingsCommand { get; }
    public IAsyncRelayCommand GoToHowToCommand { get; }
    
    private readonly GameConfig _gameConfig;

    public MainMenuViewModel(GameConfig gameConfig)
    {
        StartClassicGameCommand = new AsyncRelayCommand(OnStartClassicGame);
        GoToAchievementsCommand = new AsyncRelayCommand(OnGoToAchievements);
        GoToGameModesCommand = new AsyncRelayCommand(OnGoToGameModes);
        GoToSettingsCommand = new AsyncRelayCommand(OnGoToSettings);
        GoToHowToCommand = new AsyncRelayCommand(OnGoToHowTo);

        _gameConfig = gameConfig;
    }

    private async Task OnStartClassicGame()
    {
        await Shell.Current.GoToAsync("///GamePage");
    }
    private async Task OnGoToAchievements()
    {
        await Shell.Current.GoToAsync("///AchievementsPage");
    }

    private async Task OnGoToGameModes()
    {
        await Shell.Current.GoToAsync("///GameModesPage");
    }

    private async Task OnGoToSettings()
    {
        await Shell.Current.GoToAsync("///SettingsPage");
    }

    private async Task OnGoToHowTo()
    {
        await Shell.Current.GoToAsync("///HowToPage");
    }
}
