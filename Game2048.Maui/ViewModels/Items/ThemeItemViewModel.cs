using CommunityToolkit.Mvvm.ComponentModel;
using Game2048.Maui.Enums;

namespace Game2048.Maui.ViewModels.Items;

public sealed partial class ThemeItemViewModel : ObservableObject
{
    public required GameTheme Theme { get; init; }

    public required string ThemeTitle { get; init; }

    public required string ThemeDesc { get; init; }

    public required int Price { get; init; }

    public required IReadOnlyList<Color> PreviewColors { get; init; }

    private bool _isUnlocked = false;
    public bool IsUnlocked
    {
        get { return _isUnlocked; }
        set { _isUnlocked = value; OnPropertyChanged(); }
    }

    private bool _isSelected = false;
    public bool IsSelected 
    { 
        get { return _isSelected; } 
        set { _isSelected = value; OnPropertyChanged(); } 
    }

}
