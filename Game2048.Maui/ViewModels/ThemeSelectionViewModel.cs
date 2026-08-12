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
    private readonly IStoreManager _storeManager;
    public ObservableCollection<ThemeItemViewModel> ThemeItems { get; private set; } = new();
    public ThemePreviewViewModel PreviewViewModel { get; } = new();
    public PurchaseThemeOverlayViewModel PurchaseOverlayViewModel { get; } = new();
    public GameTheme SelectedTheme { get; private set; }
    public IRelayCommand SelectThemeCommand { get; private set; }
    public IRelayCommand PreviewThemeCommand { get; private set; }

    public event Action? ThemePreviewRequested; //
    public event Action? ThemePurchaseRequested; //
    public event Action? ThemePurchaseSucceeded; //
    public event Action? InsufficientCoinsOccurred; //

    private readonly Action<int> _onCoinsChangedHandler;//save the subscription handler to unsub in Dispose()

    public ThemeItemViewModel? SelectedThemeItem => ThemeItems.FirstOrDefault(th => th.IsSelected);
    public int CurrentCoins => _profileManager.CurrentCoins;

    public ThemeSelectionViewModel(IThemesManager themesManager, IProfileManager profileManager, IStoreManager storeManager)
    {
        _themesManager = themesManager;
        _profileManager = profileManager;
        _storeManager = storeManager;
        SelectedTheme = _themesManager.CurrentTheme;

        _onCoinsChangedHandler = _ => OnPropertyChanged(nameof(CurrentCoins));
        _profileManager.CoinsChanged += _onCoinsChangedHandler;
        _profileManager.ThemeUnlocked += OnThemeUnlocked;//

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

        SortAndSetThemes();
    }

    private void SortAndSetThemes()
    {
        var sorted = ThemeItems
            .OrderByDescending(t => t.IsUnlocked)
            .ThenBy(t => t.Price)
            .ToList();

        ThemeItems.Clear();
        foreach (var item in sorted)
        {
            ThemeItems.Add(item);
        }

        OnPropertyChanged(nameof(SelectedThemeItem));
    }

    private void SetSelectedThemeItem(GameTheme theme)
    {
        SelectedTheme = theme;

        foreach (var item in ThemeItems)
        {
            item.IsSelected = (item.Theme == theme);
        }

        OnPropertyChanged(nameof(SelectedThemeItem));
    }

    private void OnThemeUnlocked(GameTheme theme)
    {
        var themeItem = ThemeItems.FirstOrDefault(th => th.Theme == theme);
        if (themeItem is not null)
        {
            themeItem.IsUnlocked = true;
        }

        SortAndSetThemes();
    }

    private void OnSelectTheme(ThemeItemViewModel? theme)
    {
        if (theme is null) return;

        bool isUnlocked = _profileManager.IsThemeUnlocked(theme.Theme);

        if (isUnlocked)
        {
            _ = _themesManager.ApplyThemeAsync(theme.Theme);
            SetSelectedThemeItem(theme.Theme);
        }
        else
        {
            PurchaseOverlayViewModel.ThemeItem = theme;
            ThemePurchaseRequested?.Invoke();
        }
    }

    private async void OnTryBuyAndApplyTheme()
    {
        if (PurchaseOverlayViewModel.ThemeItem is null) return;

        var themeToBuy = PurchaseOverlayViewModel.ThemeItem.Theme;

        bool isPurchased = _storeManager.TryBuyTheme(themeToBuy);
        
        if (isPurchased)
        {
            try
            {
                await _profileManager.SaveCurrentProfileAsync();
                await _themesManager.ApplyThemeAsync(themeToBuy);
                SetSelectedThemeItem(themeToBuy);
                SortAndSetThemes();
                ThemePurchaseSucceeded?.Invoke();
            }
            catch (Exception ex)
            {
                // ILogger coming soon i guess
            }
        }
        else
        {
            InsufficientCoinsOccurred?.Invoke();
        }
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
        _profileManager.CoinsChanged -= _onCoinsChangedHandler;
        _profileManager.ThemeUnlocked -= OnThemeUnlocked;
    }
}
