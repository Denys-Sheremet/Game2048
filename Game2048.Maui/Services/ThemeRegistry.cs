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
        { GameTheme.PinkyPinkTheme, new PinkyPinkTheme()},
        { GameTheme.RoyalRedTheme, new RoyalRedTheme()},
        { GameTheme.RoyalBlueTheme, new RoyalBlueTheme()},
        { GameTheme.HazardTheme, new HazardTheme()},
        { GameTheme.SolarTheme, new SolarTheme()},
        { GameTheme.TokyoTheme, new TokyoTheme()},
        { GameTheme.MatchaCreamTheme, new MatchaCreamTheme()},
        { GameTheme.TerminalRetroTheme, new TerminalRetroTheme()},
        { GameTheme.AbyssTheme, new AbyssTheme()},
        { GameTheme.VaporWaveTheme, new VaporWaveTheme()},
        { GameTheme.TheGreatCityTheme, new TheGreatCityTheme()},
        { GameTheme.NebulaTheme, new NebulaTheme()},
        { GameTheme.Cyberpunk2078Theme, new Cyberpunk2078Theme()},
        { GameTheme.SakuraTheme, new SakuraTheme()},
        { GameTheme.BayHarbourTheme, new BayHarbourTheme()},
        { GameTheme.TetrisNumberlessTheme, new TetrisNumberlessTheme()},
        { GameTheme.EmeraldBloomNumberlessTheme, new EmeraldBloomNumberlessTheme()},
        { GameTheme.RarityNumberlessTheme, new RarityNumberlessTheme()},
        { GameTheme.MonochromeNumberlessTheme, new MonochromeNumberlessTheme()}
    };

    public ResourceDictionary this[GameTheme theme] => Themes[theme];

    public IEnumerable<GameTheme> GetThemes() { return Themes.Keys; }
}
