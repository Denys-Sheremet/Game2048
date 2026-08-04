using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.ViewModels.Items;

public partial class ThemePreviewTileViewModel : ObservableObject
{
    public int Value { get; }

    private Color _backgroundColor;
    public Color BackgroundColor
    {
        get => _backgroundColor;
        set => SetProperty(ref _backgroundColor, value);
    }

    private Color _textColor;
    public Color TextColor
    {
        get => _textColor;
        set => SetProperty(ref _textColor, value);
    }

    private Color _borderColor;
    public Color BorderColor
    {
        get => _borderColor;
        set => SetProperty(ref _borderColor, value);
    }

    public ThemePreviewTileViewModel(
        int value,
        Color backgroundColor,
        Color textColor,
        Color borderColor)
    {
        Value = value;
        _backgroundColor = backgroundColor;
        _textColor = textColor;
        _borderColor = borderColor;
    }
}
