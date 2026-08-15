using CommunityToolkit.Mvvm.ComponentModel;
using Game2048.Maui.Models;
using Game2048.Maui.Resources.Styles.Themes;

namespace Game2048.Maui.ViewModels.Items;

public partial class ThemePreviewViewModel : ObservableObject
{
    private static readonly int[] PreviewTileValues = [2, 16, 32, 64];

    private string _themeTitle = string.Empty;
    public string ThemeTitle
    {
        get => _themeTitle;
        set => SetProperty(ref _themeTitle, value);
    }

    private ThemePreviewColors? _previewColors;
    public ThemePreviewColors? PreviewColors
    {
        get => _previewColors;
        set
        {
            if (SetProperty(ref _previewColors, value))
            {
                OnPropertyChanged(nameof(GameBackgroundColor));
                OnPropertyChanged(nameof(GameFieldBackgroundColor));
                OnPropertyChanged(nameof(EmptyCellColor));
                OnPropertyChanged(nameof(TitleColor));
                RebuildPreviewTiles(value);
            }
        }
    }
    
    public Color TitleColor => PreviewColors?.GetColor(ThemeResourceKeys.MainTitleColor) ?? Colors.Transparent;
    public Brush GameBackgroundColor => PreviewColors?.GetBrush(ThemeResourceKeys.GameBackgroundColor) ?? Colors.Transparent;
    public Brush GameFieldBackgroundColor => PreviewColors?.GetBrush(ThemeResourceKeys.GameFieldBackgroundColor) ?? Colors.Transparent;
    public Brush EmptyCellColor => PreviewColors?.GetBrush(ThemeResourceKeys.EmptyCellColor) ?? Colors.Transparent;

    private IReadOnlyList<ThemePreviewTileViewModel> _previewTiles = [];
    public IReadOnlyList<ThemePreviewTileViewModel> PreviewTiles
    {
        get => _previewTiles;
        private set => SetProperty(ref _previewTiles, value);
    }

    public ThemePreviewViewModel(){}

    public void SetTheme(string title, ThemePreviewColors colors)
    {
        ThemeTitle = title;
        PreviewColors = colors;
    }

    private void RebuildPreviewTiles(ThemePreviewColors? colors)
    {
        if (colors is null)
        {
            PreviewTiles = [];
            return;
        }

        var tiles = new List<ThemePreviewTileViewModel>(PreviewTileValues.Length);

        foreach (var val in PreviewTileValues)
        {
            var bg = colors.GetBrush(ThemeResourceKeys.TileBackgroundColor(val));
            var text = colors.GetColor(ThemeResourceKeys.TileTextColor(val));
            var border = colors.GetColor(ThemeResourceKeys.TileBorderColor(val));

            tiles.Add(new ThemePreviewTileViewModel(val, bg, text, border));
        }

        PreviewTiles = tiles;
    }
}
