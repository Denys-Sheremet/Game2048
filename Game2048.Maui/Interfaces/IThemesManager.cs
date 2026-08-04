using Game2048.Maui.Enums;
using Game2048.Maui.Models;

namespace Game2048.Maui.Interfaces;

public interface IThemesManager
{
    GameTheme CurrentTheme { get;}
    GameTheme GetThemeFromString(string str);

    void ApplyTheme(GameTheme theme);

    Color GetThemeColor(string resourceKey);

    Color GetThemeColor(string resourceKey, GameTheme theme);

    IEnumerable<GameTheme> GetAllThemes();

    List<Color> GetPreviewColors(GameTheme theme, int count = 4);

    ThemePreviewColors GetThemePreviewColors(GameTheme theme);
}
