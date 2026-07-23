using Game2048.Core.Enums;
using Game2048.Core.Models;
using Game2048.Maui.Enums;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Resources.Localization;
using System.Globalization;

namespace Game2048.Maui.Services;

public class SettingsManager : ISettingsManager
{
    private readonly GameConfig _config;
    private readonly IThemesManager _themesManager;
    public GameTheme CurrentTheme { get; private set; }

    private const string KeyGameMode = "game_mode";
    private const string KeyLanguage = "app_lang";
    private const string KeyTheme = "app_theme";

    public string GetCurrentLang() => Preferences.Default.Get(KeyLanguage, "en");

    public SettingsManager(GameConfig config, IThemesManager themesManager)
    {
        _config = config;
        _themesManager = themesManager;

        string themeStr = Preferences.Default.Get(KeyTheme, nameof(GameTheme.ClassicTheme));

        CurrentTheme = _themesManager.GetThemeFromString(themeStr);
    }

    public bool LoadInitialSettings()
    {
        var modeStr = Preferences.Default.Get(KeyGameMode, GameModeType.Classic.ToString());
        if (Enum.TryParse<GameModeType>(modeStr, out var mode))
        {
            _config.SetConfig(mode);
        }
        else
        {
            _config.SetConfig(GameModeType.Classic);
        }

        if (CurrentTheme != GameTheme.ClassicTheme)
        {
            _themesManager.ApplyTheme(CurrentTheme);
        }

        if (Preferences.Default.ContainsKey(KeyLanguage))
        {
            var langStr = Preferences.Default.Get(KeyLanguage, "en");
            var culture = new CultureInfo(langStr);

            ApplyCulture(culture);

            return true;
        }

        return false;
    }

    public void SetGameMode(GameModeType mode)
    {
        if (_config.GameMode == mode) return;

        Preferences.Default.Set(KeyGameMode, mode.ToString());
        _config.SetConfig(mode);
    }

    public void SetTheme(GameTheme theme)
    {
        if (CurrentTheme == theme) return;
        CurrentTheme = theme;

        _themesManager.ApplyTheme(CurrentTheme);
    }

    public void SaveLastTheme(GameTheme theme)
    {
        var currentTheme = Preferences.Default.Get(KeyTheme, string.Empty);

        string themeStr = theme.ToString();

        if (currentTheme == themeStr)
        {
            return;
        }

        Preferences.Default.Set(KeyTheme, themeStr);
    }

    public void SetLanguage(string langCode)
    {
        var currentLang = Preferences.Default.Get(KeyLanguage, string.Empty);
        if (currentLang == langCode)
        {
            return;
        }

        Preferences.Default.Set(KeyLanguage, langCode);

        var culture = new CultureInfo(langCode);

        ApplyCulture(culture);
    }

    private void ApplyCulture(CultureInfo culture)
    {
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        AppResources.Culture = culture;
    }
}
