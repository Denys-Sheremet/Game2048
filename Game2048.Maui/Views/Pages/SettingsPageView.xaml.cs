using Game2048.Maui.Interfaces;
using Game2048.Maui.ViewModels;
using Microsoft.Maui.Controls;

namespace Game2048.Maui.Views.Pages;

public partial class SettingsPageView : ContentPage
{
    private readonly ISettingsManager _settingsManager;
    private readonly SettingsViewModel _viewModel;
    public SettingsPageView(ISettingsManager settingsManager, SettingsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _settingsManager = settingsManager;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.ResetToCurrentSettings();

        Dispatcher.Dispatch(async () =>
        {
            int currentColumn = GetCurrentSelectedLangColumn();

            SelectedLangStroke.IsVisible = true;
            Grid.SetColumn(SelectedLangStroke, currentColumn);

            PageContainer.TranslationX = -Width;
            PageContainer.Opacity = 0;

            await Task.WhenAll(
                PageContainer.TranslateToAsync(0, 0, 300, Easing.CubicOut),
                PageContainer.FadeToAsync(1, 300, Easing.CubicOut)
            );
        });
    }

    private int GetCurrentSelectedLangColumn()
    {
        string lang = _settingsManager.GetCurrentLang();

        int currentColumn = lang switch
        {
            "uk" => Grid.GetColumn(Uk_Btn),
            "en" => Grid.GetColumn(En_Btn),
            "cs" => Grid.GetColumn(Cs_Btn),
            "de" => Grid.GetColumn(De_Btn),
            _ => 0
        };

        return currentColumn;
    }

    public async void OnGoToThemeSelect(object sender, EventArgs e)
    {
        await GoToThemeSelectBtn.ScaleToAsync(0.95, 100, Easing.CubicIn);
        await GoToThemeSelectBtn.ScaleToAsync(1.0, 100, Easing.CubicOut);

        await Task.WhenAll
            (
                PageContainer.TranslateToAsync(Width, 0, 250, Easing.CubicIn),
                PageContainer.FadeToAsync(0, 250, Easing.Linear)
            );

        await Shell.Current.GoToAsync(nameof(ThemeSelectionView), false);
    }

    public async void OnBtnClicked(object sender, EventArgs e)
	{
		if (sender is Button btn)
		{
			await btn.ScaleToAsync(0.9, 100, Easing.CubicIn);
			await btn.ScaleToAsync(1.0, 100, Easing.CubicOut);
		} 
	}

    public async void OnLangSelectBtnClicked(object sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            await btn.ScaleToAsync(0.9, 100, Easing.CubicIn);
            await btn.ScaleToAsync(1.0, 100, Easing.CubicOut);
        }

        if (!_viewModel.IsNotCurrentLang) return;

        PageOverlayContainer.IsVisible = true;

        ConfirmationOverlay.IsVisible = true;
        ConfirmationOverlay.Opacity = 0;
        ConfirmationOverlay.Scale = 0.5;

        await Task.WhenAll
            (
                ConfirmationOverlay.ScaleToAsync(1.0, 150, Easing.CubicIn),
                ConfirmationOverlay.FadeToAsync(1.0, 150, Easing.CubicIn)
            );
    }

    public async void OnImgBtnClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton btn)
        {
            await btn.FadeToAsync(0.6, 200, Easing.CubicIn);

			await Task.WhenAll(
                btn.FadeToAsync(1.0, 100, Easing.CubicOut),
                TranslateStrokeToSelected(btn)
            );
        }
    }

	private async Task TranslateStrokeToSelected(ImageButton btn)
	{
		if (btn.Parent is Border parentBorder)
		{
            await SelectedLangStroke.FadeToAsync(0.0, 100, Easing.CubicIn);
            Grid.SetColumn(SelectedLangStroke, Grid.GetColumn(parentBorder));
            await SelectedLangStroke.FadeToAsync(1.0, 100, Easing.CubicIn);
        }   
    }

    public async void OnBackToMenu(object sender, EventArgs e)
	{
        await Task.WhenAll
            (
                PageContainer.TranslateToAsync(Width, 0, 250, Easing.CubicIn),
                PageContainer.FadeToAsync(0, 250, Easing.Linear)
            );
        await Task.WhenAll
            (
                Shell.Current.GoToAsync("///MainMenuPage", false)
            );
    }

    protected override bool OnBackButtonPressed() => true;
}