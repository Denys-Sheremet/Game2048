using Game2048.Maui.Enums;
using Game2048.Maui.Models;

namespace Game2048.Maui.Interfaces;

public interface IThemesManager
{
    GameTheme CurrentTheme { get;}
    GameTheme GetThemeFromString(string str);

    public event Func<Task>? ThemeChangeRequested; //event to notify subscribers that theme change is requested
    public event Action? ThemeChanged; //event to notify subscribers that theme has been changed

    Task ApplyThemeAsync(GameTheme theme);

    Brush GetCurrentThemeBrush(string resourceKey);

    Color GetThemeColor(string resourceKey, GameTheme theme);

    IEnumerable<GameTheme> GetAllThemes();

    List<Color> GetPreviewColors(GameTheme theme, int count = 4);

    ThemePreviewColors GetThemePreviewColors(GameTheme theme);
}
