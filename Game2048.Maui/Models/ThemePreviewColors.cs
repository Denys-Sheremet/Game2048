using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.Models;

public record ThemePreviewColors(IReadOnlyDictionary<string, Color> ThemeColors)
{
    public Color this[string key] => ThemeColors.TryGetValue(key, out var color) ? color : Colors.Transparent;
}
