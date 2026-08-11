using Game2048.Maui.Interfaces;
using Game2048.Maui.ViewModels;
using Game2048.Maui.ViewModels.Items;

namespace Game2048.Maui.Views.Pages;

public partial class ThemeSelectionView : ContentPage
{
    private readonly ThemeSelectionViewModel _viewModel;
    private readonly IThemesManager _themesManager;
	public ThemeSelectionView(ThemeSelectionViewModel viewModel, IThemesManager themesManager)
	{
		InitializeComponent();
        _viewModel = viewModel;
        _themesManager = themesManager;
        BindingContext = _viewModel;

        PreviewOverlay.BindingContext = _viewModel.PreviewViewModel;
        PurchaseOverlay.BindingContext = _viewModel.PurchaseOverlayViewModel;
    }

    private void SetOverlayVisibility(bool isVisible) 
    {
        PreviewOverlay.IsVisible = isVisible;
        PurchaseOverlay.IsVisible = isVisible;
        UnsuccessfulPurchaseOverlay.IsVisible = isVisible;
        PageOverlayContainer.IsVisible = isVisible;

        if (!isVisible)
        {
            PreviewOverlay.Opacity = 1.0;
            PreviewOverlay.Scale = 1.0;
            PurchaseOverlay.Opacity = 1.0;
            PurchaseOverlay.Scale = 1.0;
            UnsuccessfulPurchaseOverlay.Opacity = 1.0;
            UnsuccessfulPurchaseOverlay.Scale = 1.0;
        }
    }

    private async Task OnThemeChangeRequested()
    {
        PageContainer.IsEnabled = false;
        await PageContainer.FadeToAsync(0.5, 200, Easing.CubicOut);
    }

    private async void OnThemeChanged()
    {
        PageContainer.IsEnabled = true;
        await PageContainer.FadeToAsync(1.0, 250, Easing.CubicOut);
    }


    public async void OnBack(object sender, EventArgs e)
	{
        await Task.WhenAll
            (
                PageContainer.TranslateToAsync(Width, 0, 250, Easing.CubicIn),
                PageContainer.FadeToAsync(0, 250, Easing.Linear)
            );
        await Shell.Current.GoToAsync("..", false);
    }

    public async void OnShowThemePreviewAsync()
    {
        await ShowThemePreviewAsync();
    }

    public async void OnShowPurchaseOverlay()
    {
        PageOverlayContainer.IsVisible = true;
        PageOverlayContainer.Opacity = 0.0;
        PurchaseOverlay.IsVisible = true;
        PurchaseOverlay.Opacity = 0.0;
        PurchaseOverlay.Scale = 0.8;

        await Task.WhenAll
            (
                PageOverlayContainer.FadeToAsync(1.0, 180, Easing.CubicOut),
                PurchaseOverlay.FadeToAsync(1.0, 180, Easing.CubicOut),
                PurchaseOverlay.ScaleToAsync(1.0, 180, Easing.CubicOut)
            );
    }

    public async Task ShowThemePreviewAsync()
    {
        PageOverlayContainer.IsVisible = true;
        PageOverlayContainer.Opacity = 0.0;
        PreviewOverlay.IsVisible = true;
        PreviewOverlay.Opacity = 0.0;
        PreviewOverlay.Scale = 0.8;

        await Task.WhenAll
            (
                PageOverlayContainer.FadeToAsync(1.0, 180, Easing.CubicOut),
                PreviewOverlay.FadeToAsync(1.0, 300, Easing.CubicOut),
                PreviewOverlay.ScaleToAsync(1.0, 300, Easing.CubicOut)
            );
    }

    public void OnHideThemePreview()
    {
        _ = HideThemePreviewAsync();
    }

    public async Task HideThemePreviewAsync()
    {
        await Task.WhenAll(
            PageOverlayContainer.FadeToAsync(0, 140, Easing.CubicIn),
            PreviewOverlay.FadeToAsync(0, 140, Easing.CubicIn),
            PreviewOverlay.ScaleToAsync(0.85, 140, Easing.CubicIn)
        );

        SetOverlayVisibility(false);
    }

    public async void OnHidePurchaseOverlay()
    {
        await Task.WhenAll(
            PageOverlayContainer.FadeToAsync(0, 140, Easing.CubicIn),
            PurchaseOverlay.FadeToAsync(0, 140, Easing.CubicIn),
            PurchaseOverlay.ScaleToAsync(0.85, 140, Easing.CubicIn)
        );
        SetOverlayVisibility(false);
    }

    public async void OnHideUnsuccessfulPurchaseOverlay()
    {
        await Task.WhenAll(
            PageOverlayContainer.FadeToAsync(0, 140, Easing.CubicIn),
            UnsuccessfulPurchaseOverlay.FadeToAsync(0, 140, Easing.CubicIn),
            UnsuccessfulPurchaseOverlay.ScaleToAsync(0.85, 140, Easing.CubicIn)
        );
        SetOverlayVisibility(false);
    }

    public async void OnInsufficientCoinsOccurred()
    {
        PageOverlayContainer.IsVisible = true;
        UnsuccessfulPurchaseOverlay.IsVisible = true;
        UnsuccessfulPurchaseOverlay.Opacity = 0.0;
        UnsuccessfulPurchaseOverlay.Scale = 0.85;

        await Task.WhenAll
            (
                PurchaseOverlay.FadeToAsync(0, 140, Easing.CubicIn),
                PurchaseOverlay.ScaleToAsync(0.85, 140, Easing.CubicIn)
            );
        PurchaseOverlay.IsVisible = false;

        await Task.WhenAll
            (
                PageOverlayContainer.FadeToAsync(1.0, 180, Easing.CubicOut),
                UnsuccessfulPurchaseOverlay.FadeToAsync(1.0, 180, Easing.CubicOut),
                UnsuccessfulPurchaseOverlay.ScaleToAsync(1.0, 180, Easing.CubicOut)
            );
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.ThemePreviewRequested += OnShowThemePreviewAsync;
        _viewModel.ThemePurchaseRequested += OnShowPurchaseOverlay;
        _viewModel.ThemePurchaseSucceeded += OnHidePurchaseOverlay;
        _viewModel.InsufficientCoinsOccurred += OnInsufficientCoinsOccurred;
        PreviewOverlay.HideThemePreviewRequested += OnHideThemePreview;
        PurchaseOverlay.HidePurchaseOverlayRequested += OnHidePurchaseOverlay;
        UnsuccessfulPurchaseOverlay.HideUnsuccessfulPurchaseOverlayRequested += OnHideUnsuccessfulPurchaseOverlay;
        _themesManager.ThemeChangeRequested += OnThemeChangeRequested;
        _themesManager.ThemeChanged += OnThemeChanged;

        PageContainer.Opacity = 0;
        ThemeCollectionView.Opacity = 0;

        var selectedItem = _viewModel.SelectedThemeItem;
        ScrollThemeCollectionViewTo(selectedItem);

        await PageContainer.FadeToAsync(1, 600, Easing.CubicOut);
        await ThemeCollectionView.FadeToAsync(1.0, 400, Easing.CubicOut);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _viewModel.ThemePreviewRequested -= OnShowThemePreviewAsync;
        _viewModel.ThemePurchaseRequested -= OnShowPurchaseOverlay;
        _viewModel.ThemePurchaseSucceeded -= OnHidePurchaseOverlay;
        _viewModel.InsufficientCoinsOccurred -= OnInsufficientCoinsOccurred;
        _themesManager.ThemeChangeRequested -= OnThemeChangeRequested;
        _themesManager.ThemeChanged -= OnThemeChanged;
        PreviewOverlay.HideThemePreviewRequested -= OnHideThemePreview;
        PurchaseOverlay.HidePurchaseOverlayRequested -= OnHidePurchaseOverlay;
        UnsuccessfulPurchaseOverlay.HideUnsuccessfulPurchaseOverlayRequested -= OnHideUnsuccessfulPurchaseOverlay;

        SetOverlayVisibility(false);
    }

    private void ScrollThemeCollectionViewTo(ThemeItemViewModel? item)
    {
        Dispatcher.Dispatch(() =>
        {
            if (item is not null)
            {
                ThemeCollectionView.ScrollTo(item, position: ScrollToPosition.Center, animate: false);
            }
        });
    }

    protected override bool OnBackButtonPressed() => true;
}