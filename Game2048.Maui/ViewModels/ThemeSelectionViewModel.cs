using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Game2048.Maui.Enums;
using Game2048.Maui.Extensions;
using Game2048.Maui.Interfaces;
using Game2048.Maui.ViewModels.Items;
using System.Collections.ObjectModel;

namespace Game2048.Maui.ViewModels;

public partial class ThemeSelectionViewModel : ObservableObject, IDisposable
{
    private readonly IThemesManager _themesManager;
    private readonly IProfileManager _profileManager;
    public ObservableCollection<ThemeItemViewModel> ThemeItems { get; private set; } = new();
    public ThemePreviewViewModel PreviewViewModel { get; } = new();
    public PurchaseThemeOverlayViewModel PurchaseOverlayViewModel { get; } = new();
    public GameTheme SelectedTheme { get; private set; }
    public IRelayCommand SelectThemeCommand { get; private set; }
    public IRelayCommand PreviewThemeCommand { get; private set; }

    public event Action? ThemePreviewRequested; //
    public event Action? ThemePurchaseRequested; //
    public event Action<GameTheme>? ThemeChanged; //
    public event Action? InsufficientCoinsOccurred; //

    private readonly Action<int> _onCoinsChangedHandler;//save the subscription handler to unsub in Dispose()

    public ThemeItemViewModel? SelectedThemeItem => ThemeItems.FirstOrDefault(th => th.IsSelected);
    public int CurrentCoins => _profileManager.CurrentCoins;

    public ThemeSelectionViewModel(IThemesManager themesManager, IProfileManager profileManager)
    {
        _themesManager = themesManager;
        _profileManager = profileManager;
        SelectedTheme = _themesManager.CurrentTheme;

        _onCoinsChangedHandler = _ => OnPropertyChanged(nameof(CurrentCoins));
        _profileManager.OnCoinsChanged += _onCoinsChangedHandler;

        SelectThemeCommand = new RelayCommand<ThemeItemViewModel>(OnSelectTheme);
        PreviewThemeCommand = new RelayCommand<ThemeItemViewModel>(OnOpenPreview);
        PurchaseOverlayViewModel.BuyCommand = new RelayCommand(OnTryBuyAndApplyTheme);

        InitializeThemes();
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
                IsSelected = theme == SelectedTheme,
                SelectCommand = SelectThemeCommand,
                PreviewCommand = PreviewThemeCommand
            });
        }
    }

    

    private void OnSelectTheme(ThemeItemViewModel? theme)
    {
        if (theme is null) return;

        var unlockedThemes = _profileManager.GetUnlockedThemes();
        bool isUnlocked = unlockedThemes.Contains(theme.Theme);

        if (isUnlocked)
        {
            _ = _themesManager.ApplyThemeAsync(theme.Theme);
        }
        else
        {
            PurchaseOverlayViewModel.ThemeItem = theme;
            ThemePurchaseRequested?.Invoke();
        }
    }

    private void OnTryBuyAndApplyTheme()
    {
        //further checks from special manager
    }

    private void OnOpenPreview(ThemeItemViewModel? item)
    {
        if (item is null) return;

        var colors = _themesManager.GetThemePreviewColors(item.Theme);
        PreviewViewModel.SetTheme(item.ThemeTitle, colors);

        ThemePreviewRequested?.Invoke();
    }

    public void Dispose()
    {
        _profileManager.OnCoinsChanged -= _onCoinsChangedHandler;
    }
}
