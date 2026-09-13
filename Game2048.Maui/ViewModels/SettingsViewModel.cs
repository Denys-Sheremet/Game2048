using Game2048.Maui.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Game2048.Maui.Constants;
using Microsoft.Maui.ApplicationModel;

namespace Game2048.Maui.ViewModels;

public partial class SettingsViewModel : BindableObject
{
    public const int MaxNameLength = 14;
    private const string DefaultPlayerName = "Player";

    private readonly IProfileManager _profileManager;
    private readonly ISettingsManager _settingsManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SettingsViewModel> _logger;

    private string _playerName = string.Empty;
    private string _currentProfileName = string.Empty;
    private string _selectedLanguage = "en";

    public bool NameIsUnsaved => (PlayerName.Trim() ?? string.Empty) != CurrentProfileName;
    public bool IsNotCurrentLang => _selectedLanguage != _settingsManager.GetCurrentLang();

    public bool IsConfirmationNeeded = false;

    public SettingsViewModel(IProfileManager profileManager, ISettingsManager settingsManager, IServiceProvider serviceProvider, ILogger<SettingsViewModel> logger)
    {
        _profileManager = profileManager;
        _settingsManager = settingsManager;
        _serviceProvider = serviceProvider;
        _logger = logger;

        SaveNameCommand = new AsyncRelayCommand(SaveNameAsync);
        SelectLanguageCommand = new RelayCommand<string>(SelectLanguage);
        ConfirmAndRestartCommand = new AsyncRelayCommand(ConfirmAndRestart);
        GitHubLinkCommand = new AsyncRelayCommand(OpenGitHubLinkAsync);
        PrivacyPolicyLinkCommand = new AsyncRelayCommand(OpenPrivacyPolicyLinkAsync);
        SendEmailCommand = new AsyncRelayCommand(SendEmailAsync);

        ResetToCurrentSettings();
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
                OnPropertyChanged(nameof(NameIsUnsaved));
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
                OnPropertyChanged(nameof(NameIsUnsaved));
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
                OnPropertyChanged(nameof(IsNotCurrentLang));
                OnPropertyChanged();
            }
        }
    }

    public IAsyncRelayCommand SaveNameCommand { get; private set; }
    public IRelayCommand SelectLanguageCommand { get; private set; }
    public IAsyncRelayCommand ConfirmAndRestartCommand { get; private set; }
    public IAsyncRelayCommand GitHubLinkCommand { get; private set; }
    public IAsyncRelayCommand PrivacyPolicyLinkCommand { get; private set; }
    public IAsyncRelayCommand SendEmailCommand { get; private set; }

    private async Task SaveNameAsync() 
    {
        if (!NameIsUnsaved) return;

        string finalName = ValidateName(PlayerName);

        _profileManager.SetPlayerName(finalName);
        await _profileManager.SaveCurrentProfileAsync();

        CurrentProfileName = finalName; 
        PlayerName = finalName;
    }

    private string ValidateName(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
        {
            return CurrentProfileName ?? DefaultPlayerName;
        }

        string trimmed = rawName.Trim();

        if (trimmed.Length == 0)
        {
            return CurrentProfileName ?? DefaultPlayerName;
        }

        return trimmed;
    }

    private void SelectLanguage(string? langCode) 
    {
        if (langCode is null || SelectedLanguage == langCode) return;
        SelectedLanguage = langCode;

        OnPropertyChanged(nameof(IsNotCurrentLang));
    }

    private async Task ConfirmAndRestart()
    {
        await _profileManager.SaveCurrentProfileAsync();

        _settingsManager.SetLanguage(SelectedLanguage);

        if (Application.Current is App app) 
        {
            app.RestartApp();
        }
    }

    public void ResetToCurrentSettings()
    {
        CurrentProfileName = _profileManager.CurrentProfile?.Name ?? DefaultPlayerName;
        PlayerName = CurrentProfileName;
        SelectedLanguage = _settingsManager.GetCurrentLang();
    }

    private async Task OpenGitHubLinkAsync()
    {
        try
        {
            await Browser.Default.OpenAsync(AppConstants.GitHubUrl, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to open GitHub link.");
        }
    }

    private async Task OpenPrivacyPolicyLinkAsync()
    {
        try
        {
            await Browser.Default.OpenAsync(AppConstants.PrivacyPolicyUrl, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to open Privacy Policy link.");
        }
    }

    private async Task SendEmailAsync()
    {
        try
        {
            string subject = Uri.EscapeDataString("Game 2048 - Support / Bug Report");
            string mailtoUri = $"mailto:{AppConstants.SupportEmail}?subject={subject}";

            await Launcher.Default.OpenAsync(new Uri(mailtoUri));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to open email client.");
        }
    }
}
