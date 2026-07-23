using Game2048.Maui.Interfaces;
using Game2048.Maui.Services;
using CommunityToolkit.Mvvm.Input;
namespace Game2048.Maui.ViewModels;

public partial class SettingsViewModel : BindableObject
{
    private readonly IProfileManager _profileManager;
    private readonly ISettingsManager _settingsManager;
    private readonly IServiceProvider _serviceProvider;

    private string _playerName = string.Empty;
    private string _currentProfileName = string.Empty;
    private string _selectedLanguage = "en";

    public SettingsViewModel(IProfileManager profileManager, ISettingsManager settingsManager, IServiceProvider serviceProvider)
    {
        _profileManager = profileManager;
        _settingsManager = settingsManager;
        _serviceProvider = serviceProvider;

        SaveNameCommand = new AsyncRelayCommand(SaveNameAsync);
        SelectLanguageCommand = new RelayCommand<string>(SelectLanguage);
        NavigateToThemeSelectCommand = new AsyncRelayCommand(NavigateToThemeSelectAsync);
        ApplySettingsCommand = new AsyncRelayCommand(ApplySettingsAsync);

        CurrentProfileName = _profileManager.CurrentProfile?.Name ?? "Player";
        PlayerName = CurrentProfileName;
        SelectedLanguage = "en"; //
    }

    public string PlayerName
    {
        get => _playerName;
        set
        {
            if (_playerName != value)
            {
                _playerName = value;
                OnPropertyChanged();
            }
        }
    }

    public string CurrentProfileName
    {
        get => _currentProfileName;
        set
        {
            if (_currentProfileName != value)
            {
                _currentProfileName = value;
                OnPropertyChanged();
            }
        }
    }

    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (_selectedLanguage != value)
            {
                _selectedLanguage = value;
                OnPropertyChanged();
            }
        }
    }

    public IRelayCommand SaveNameCommand { get; }
    public IRelayCommand SelectLanguageCommand { get; }
    public IRelayCommand NavigateToThemeSelectCommand { get; }
    public IRelayCommand ApplySettingsCommand { get; }

    private async Task SaveNameAsync() { }

    private void SelectLanguage(string? langCode) { }

    private async Task NavigateToThemeSelectAsync() { }

    private async Task ApplySettingsAsync() { }


}
