using Game2048.Core.Enums;
using Game2048.Core.Models;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Resources.Localization;
using System.Diagnostics;
using System.Globalization;

namespace Game2048.Maui.Services;

public class SettingsManager : ISettingsManager
{
    private readonly GameConfig _config;

    private const string KeyGameMode = "game_mode";
    private const string KeyLanguage = "app_lang";
    private const string KeyTheme = "app_theme";

    public SettingsManager(GameConfig config)
    {
        _config = config;
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

        var themeStr = Preferences.Default.Get(KeyTheme, "ClassicTheme");
        // TODO ApplyTheme(themeStr); 

        if (Preferences.Default.ContainsKey(KeyLanguage))
        {
            var langStr = Preferences.Default.Get(KeyLanguage, "en");
            var culture = new System.Globalization.CultureInfo(langStr);

            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;

            AppResources.Culture = culture;

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

    public void SetTheme(string themeName)
    {
        var currentTheme = Preferences.Default.Get(KeyTheme, string.Empty);
        if (currentTheme == themeName)
        {
            return;
        }

        Preferences.Default.Set(KeyTheme, themeName);

        // TODO: (MergedDictionaries)
    }

    public void SetLanguage(string langCode)
    {
        var currentLang = Preferences.Default.Get(KeyLanguage, string.Empty);
        //
        Debug.WriteLine(currentLang);
        //
        if (currentLang == langCode)
        {
            return;
        }

        Preferences.Default.Set(KeyLanguage, langCode);

        var culture = new CultureInfo(langCode);
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        AppResources.Culture = culture;
    }
}
