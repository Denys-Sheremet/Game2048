using Game2048.Maui.Enums;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Resources.Styles.Themes;

namespace Game2048.Maui.Services;

public class ThemesManager : IThemesManager
{
    private readonly Dictionary<GameTheme, ResourceDictionary> _themes = new()
    {
        { GameTheme.ClassicTheme, new ClassicTheme()},
        { GameTheme.DarkTheme, new DarkTheme()},
        { GameTheme.NatureTheme, new NatureTheme()},
        { GameTheme.NeonColorTheme, new NeonColorTheme()},
        { GameTheme.RainbowTheme, new RainbowTheme()},
        { GameTheme.BWTheme, new BWTheme()}
    };

    public GameTheme GetThemeFromString(string str)
    {
        if(Enum.TryParse<GameTheme>(str, out var theme)){
            return theme;
        }
        return GameTheme.ClassicTheme;
    }

    

    public void ApplyTheme(GameTheme theme) 
    {
        var themeToApply = GetResource(theme);

        ICollection<ResourceDictionary> mergedDictionaries = Application.Current!.Resources.MergedDictionaries;

        foreach (var existingTheme in mergedDictionaries.ToList())
        {
            if (_themes.Values.Contains(existingTheme))
            {
                mergedDictionaries.Remove(existingTheme);
            }
        }
        mergedDictionaries.Add(themeToApply);
    }

    private ResourceDictionary GetResource(GameTheme theme)
    {
        if(_themes.TryGetValue(theme, out var res))
        {
            return res;
        }
        return _themes[GameTheme.ClassicTheme];
    }

}
