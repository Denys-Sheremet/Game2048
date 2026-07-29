using CommunityToolkit.Mvvm.ComponentModel;
using Game2048.Maui.Enums;
using Game2048.Maui.Extensions;
using Game2048.Maui.Interfaces;
using Game2048.Maui.ViewModels.Items;
using System.Collections.ObjectModel;

namespace Game2048.Maui.ViewModels;

public partial class ThemeSelectionViewModel : ObservableObject
{
    private readonly IThemesManager _themesManager;
    public ObservableCollection<ThemeItemViewModel> ThemeItems { get; private set; }
    public GameTheme SelectedTheme { get; private set; }

    public ThemeSelectionViewModel(IThemesManager themesManager)
    {
        _themesManager = themesManager;
        ThemeItems = new();
        InitializeThemes();
    }

    private void InitializeThemes()
    {
        var themes = _themesManager.GetAllThemes();

        foreach (var theme in themes) 
        {
            List<Color> previewColors = _themesManager.GetPreviewColors(theme, 4);

            ThemeItems.Add(new()
            {
                Theme = theme,
                ThemeTitle = theme.GetTitle(),
                ThemeDesc = theme.GetDesc(),
                Price = theme.GetPrice(),
                PreviewColors = previewColors
            });
        }
    }

}
