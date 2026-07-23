using Game2048.Maui.Enums;

namespace Game2048.Maui.Interfaces;

public interface IThemesManager
{
    GameTheme GetThemeFromString(string str);
    void ApplyTheme(GameTheme theme);
}
