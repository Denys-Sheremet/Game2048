using Game2048.Maui.Enums;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Resources.Styles.Themes;

namespace Game2048.Maui.Services;

public class ThemesManager : IThemesManager
{
    private readonly Dictionary<GameTheme, ResourceDictionary> _themes = new()
    //Each theme should implement IThemeResource to be correctly worked with
    //Each theme should be registered in current dictionary
    {
        { GameTheme.ClassicTheme, new ClassicTheme()},
        { GameTheme.DarkTheme, new DarkTheme()},
        { GameTheme.NatureTheme, new NatureTheme()},
        { GameTheme.NeonColorTheme, new NeonColorTheme()},
        { GameTheme.RainbowTheme, new RainbowTheme()},
        { GameTheme.BWTheme, new BWTheme()},
        { GameTheme.PinkyPinkTheme, new PinkyPinkTheme()}
    };

    private ResourceDictionary _currentTheme;

    public ThemesManager()
    {
        _currentTheme = _themes[GameTheme.ClassicTheme];
    }

    public GameTheme GetThemeFromString(string str)
    {
        if(Enum.TryParse<GameTheme>(str, out var theme)){
            return theme;
        }
        return GameTheme.ClassicTheme;
    }

    public void ApplyTheme(GameTheme theme)
    //Each theme should implement IThemeResource to be correctly worked with
    {
        var themeToApply = GetResource(theme);

        if (ReferenceEquals(themeToApply, _currentTheme)) return;

        ICollection<ResourceDictionary> mergedDictionaries = Application.Current!.Resources.MergedDictionaries;

        if (!mergedDictionaries.Remove(_currentTheme))
        {
            var existingTheme = mergedDictionaries.FirstOrDefault(d => d is IThemeResource);
            if (existingTheme is not null)
            {
                mergedDictionaries.Remove(existingTheme);
            }
        }
        //
        //maybe some animation coming soon
        //

        mergedDictionaries.Add(themeToApply);
        _currentTheme = themeToApply;
    }

    private ResourceDictionary GetResource(GameTheme theme)
    {
        if(_themes.TryGetValue(theme, out var res))
        {
            return res;
        }
        return _themes[GameTheme.ClassicTheme];
    }

    public Color GetThemeColor(string resourceKey)
    {
        if (Application.Current?.Resources.TryGetValue(resourceKey, out var resource) == true)
        {
            if (resource is Color color)
            {
                return color;
            }
        }

        return Colors.Transparent;
    }

}
