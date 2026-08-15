using Game2048.Maui.Resources.Styles.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.Models;

public record ThemePreviewColors(IReadOnlyDictionary<string, object> ThemeColors)
{
    public Brush GetBrush(string key)
    {
        if (!ThemeColors.TryGetValue(key, out var resource))
            return new SolidColorBrush(Colors.Transparent);

        return resource switch
        {
            Brush brush => brush,
            Color color => new SolidColorBrush(color),
            _ => new SolidColorBrush(Colors.Transparent)
        };
    }

    public Color GetColor(string key)
    {
        if (!ThemeColors.TryGetValue(key, out var resource))
            return Colors.Transparent;

        return resource switch
        {
            Color color => color,
            SolidColorBrush scb => scb.Color,
            _ => Colors.Transparent
        };
    }
}
