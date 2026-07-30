using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Game2048.Maui.Enums;
using Game2048.Maui.Extensions;
using Game2048.Maui.Interfaces;
using Game2048.Maui.ViewModels.Items;
using System.Collections.ObjectModel;

namespace Game2048.Maui.ViewModels;

public partial class ThemeSelectionViewModel : ObservableObject
{
    private readonly IThemesManager _themesManager;
    private readonly IProfileManager _profileManager;
    public ObservableCollection<ThemeItemViewModel> ThemeItems { get; private set; } = new();
    public GameTheme SelectedTheme { get; private set; }
    public IAsyncRelayCommand SelectThemeCommand { get; private set; }
    public IAsyncRelayCommand PreviewThemeCommand { get; private set; }
    public event Action<GameTheme>? OnSelectedThemeChanged; //
    public event Action<GameTheme>? OnSelectedThemePreview; //
    public event Action<GameTheme>? OnSelectedThemeLocked; //

    public ThemeSelectionViewModel(IThemesManager themesManager, IProfileManager profileManager)
    {
        _themesManager = themesManager;
        _profileManager = profileManager;
        SelectedTheme = _themesManager.CurrentTheme;
        InitializeThemes();
        SelectThemeCommand = new AsyncRelayCommand<ThemeItemViewModel>(OnSelectTheme);
        PreviewThemeCommand = new AsyncRelayCommand<ThemeItemViewModel>(OnOpenPreview);
    }

    private void InitializeThemes()
    {
        var themes = _themesManager.GetAllThemes();
        var unlockedThemes = _profileManager.GetUnlockedThemes();

        foreach (var theme in themes) 
        {
            List<Color> previewColors = _themesManager.GetPreviewColors(theme, 8);
            Color previewTextColor = _themesManager.GetThemeColor("PreviewTextColor", theme);
            bool isUnlocked = unlockedThemes.Contains(theme);

            ThemeItems.Add(new()
            {
                Theme = theme,
                ThemeTitle = theme.GetTitle(),
                ThemeDesc = theme.GetDesc(),
                Price = theme.GetPrice(),
                PreviewColors = previewColors,
                PreviewTextColor = previewTextColor,
                IsUnlocked = isUnlocked,
                IsSelected = theme == SelectedTheme
            });
        }
    }

    private async Task OnSelectTheme(ThemeItemViewModel? theme)
    {

    }

    private async Task OnOpenPreview(ThemeItemViewModel? theme)
    {

    }

}
