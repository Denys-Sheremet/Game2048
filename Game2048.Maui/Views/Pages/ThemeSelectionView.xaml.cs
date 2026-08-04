using Game2048.Maui.ViewModels;
using Game2048.Maui.ViewModels.Items;

namespace Game2048.Maui.Views.Pages;

public partial class ThemeSelectionView : ContentPage
{
    private readonly ThemeSelectionViewModel _viewModel;
	public ThemeSelectionView(ThemeSelectionViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        PreviewOverlay.BindingContext = _viewModel.PreviewViewModel;

        _viewModel.ThemePreviewRequested += OnShowThemePreviewAsync;
        PreviewOverlay.HideThemePreviewRequested += OnHideThemePreview;

    }

	public async void OnBack(object sender, EventArgs e)
	{
        await Task.WhenAll
            (
                PageContainer.TranslateTo(Width, 0, 250, Easing.CubicIn),
                PageContainer.FadeTo(0, 250, Easing.Linear)
            );
        await Task.WhenAll
            (
                Shell.Current.GoToAsync("..", false)
            );
    }

    public async void OnShowThemePreviewAsync()
    {
        await ShowThemePreviewAsync();
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
                PageOverlayContainer.FadeTo(1.0, 180, Easing.CubicOut),
                PreviewOverlay.FadeTo(1.0, 180, Easing.CubicOut),
                PreviewOverlay.ScaleTo(1.0, 180, Easing.CubicOut)
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
            PageOverlayContainer.FadeTo(0, 140, Easing.CubicIn),
            PreviewOverlay.FadeTo(0, 140, Easing.CubicIn),
            PreviewOverlay.ScaleTo(0.85, 140, Easing.CubicIn)
        );

        PreviewOverlay.IsVisible = false;
        PageOverlayContainer.IsVisible = false;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        PageContainer.Opacity = 0;
        ThemeCollectionView.Opacity = 0;

        var selectedItem = _viewModel.SelectedThemeItem;
        ScrollThemeCollectionViewTo(selectedItem);

        await PageContainer.FadeTo(1, 600, Easing.CubicOut);
        await ThemeCollectionView.FadeTo(1.0, 400, Easing.CubicOut);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.ThemePreviewRequested -= OnShowThemePreviewAsync;
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