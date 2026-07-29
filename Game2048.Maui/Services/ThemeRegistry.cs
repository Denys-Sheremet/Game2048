using Game2048.Maui.Enums;
using Game2048.Maui.Resources.Styles.Themes;

namespace Game2048.Maui.Services;

public sealed class ThemeRegistry
{
    public IReadOnlyDictionary<GameTheme, ResourceDictionary> Themes { get; } =
        new Dictionary<GameTheme, ResourceDictionary>()
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

    public ResourceDictionary this[GameTheme theme] => Themes[theme];

    public IEnumerable<GameTheme> GetThemes() { return Themes.Keys; }
}
