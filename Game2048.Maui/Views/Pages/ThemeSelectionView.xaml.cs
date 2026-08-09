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
                PreviewOverlay.FadeToAsync(1.0, 180, Easing.CubicOut),
                PreviewOverlay.ScaleToAsync(1.0, 180, Easing.CubicOut)
            );
    }

    public void OnHideThemePreview()
    {
        _ = HideThemePreviewAsync();
    }

    public async Task HideThemePreviewAsync()
    {
        if (!PreviewOverlay.IsVisible) return;

        await Task.WhenAll(
            PageOverlayContainer.FadeToAsync(0, 140, Easing.CubicIn),
            PreviewOverlay.FadeToAsync(0, 140, Easing.CubicIn),
            PreviewOverlay.ScaleToAsync(0.85, 140, Easing.CubicIn)
        );

        PreviewOverlay.IsVisible = false;
        PageOverlayContainer.IsVisible = false;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.ThemePreviewRequested += OnShowThemePreviewAsync;
        _viewModel.ThemePurchaseRequested += OnShowPurchaseOverlay;
        PreviewOverlay.HideThemePreviewRequested += OnHideThemePreview;
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
        _themesManager.ThemeChangeRequested -= OnThemeChangeRequested;
        _themesManager.ThemeChanged -= OnThemeChanged;
        PreviewOverlay.HideThemePreviewRequested -= OnHideThemePreview;

        PreviewOverlay.IsVisible = false;
        PageOverlayContainer.IsVisible = false;
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