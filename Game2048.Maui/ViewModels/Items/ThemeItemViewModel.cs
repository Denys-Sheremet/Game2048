using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Game2048.Maui.Enums;

namespace Game2048.Maui.ViewModels.Items;

public sealed partial class ThemeItemViewModel : ObservableObject
{
    public required GameTheme Theme { get; init; }

    public required IRelayCommand SelectCommand { get; init; }
    public required IRelayCommand PreviewCommand { get; init; }

    public required string ThemeTitle { get; init; }

    public required string ThemeDesc { get; init; }

    public required int Price { get; init; }

    public required IReadOnlyList<Color> PreviewColors { get; init; }

    public required Color PreviewTextColor {  get; init; }

    private bool _isUnlocked = false;
    public bool IsUnlocked
    {
        get { return _isUnlocked; }
        set { _isUnlocked = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsLocked)); }
    }

    private bool _isSelected = false;
    public bool IsSelected 
    { 
        get { return _isSelected; } 
        set { _isSelected = value; OnPropertyChanged(); } 
    }

    public bool IsLocked => !IsUnlocked;

}
