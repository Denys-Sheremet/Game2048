using Game2048.Maui.Enums;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Resources.Styles.Themes;

namespace Game2048.Maui.Services;

public class ThemesManager : IThemesManager
{
    private readonly ThemeRegistry _themeRegistry;

    private ResourceDictionary _currentTheme;

    public GameTheme CurrentTheme { get; private set; }

    public ThemesManager(ThemeRegistry registry)
    {
        _themeRegistry = registry;
        CurrentTheme = GameTheme.ClassicTheme;
        _currentTheme = _themeRegistry[GameTheme.ClassicTheme];
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
        CurrentTheme = theme;
        _currentTheme = themeToApply;
    }

    private ResourceDictionary GetResource(GameTheme theme)
    {
        if(_themeRegistry.Themes.TryGetValue(theme, out var res))
        {
            return res;
        }
        return _themeRegistry[GameTheme.ClassicTheme];
    }

    public IEnumerable<GameTheme> GetAllThemes()
    {
        return _themeRegistry.GetThemes();
    }

    public Color GetThemeColor(string resourceKey)
    {
        if (Application.Current?.Resources.TryGetValue(resourceKey, out var resource) == true)
        {
            if (resource is Color color)
            {
                return color;
            }
            if (resource is SolidColorBrush brush)
            {
                return brush.Color;
            }
        }

        return Colors.Transparent;
    }

    public Color GetThemeColor(string resourceKey, GameTheme theme)
    {
        if(GetResource(theme).TryGetValue(resourceKey, out var resource))
        {
            if (resource is Color color)
            {
                return color;
            }
            if (resource is SolidColorBrush brush)
            {
                return brush.Color;
            }
        }
        return Colors.Transparent;
    }

    public List<Color> GetPreviewColors(GameTheme theme, int count = 4)
    {
        List<Color> colors = new();
        for (int i = 1; i <= count; i++)
        {
            string key = $"PreviewColor{i}";
            colors.Add(GetThemeColor(key, theme));
        }
        return colors;
    }
}
