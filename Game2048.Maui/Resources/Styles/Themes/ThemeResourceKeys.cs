using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.Resources.Styles.Themes;

public static class ThemeResourceKeys
{
    #region Background

    public const string GameBackgroundColor = nameof(GameBackgroundColor);
    public const string GameFieldBackgroundColor = nameof(GameFieldBackgroundColor);
    public const string EmptyCellColor = nameof(EmptyCellColor);
    public const string MainTitleColor = nameof(MainTitleColor);

    #endregion

    #region Tiles

    public static string TileBackgroundColor(int value)
        => $"Tile{value}BGColor";

    public static string TileTextColor(int value)
        => $"Tile{value}TextColor";

    public static string TileBorderColor(int value)
        => $"Tile{value}BorderColor";

    public const string ExtraTileBackgroundColor = "TileExtraBGColor";
    public const string ExtraTileTextColor = "TileExtraTextColor";
    public const string ExtraTileBorderColor = "TileExtraBorderColor";

    #endregion

    #region Preview

    public static string PreviewColor(int index)
        => $"PreviewColor{index}";

    #endregion
}
