using Game2048.Maui.ViewModels;

namespace Game2048.Maui.Views.Pages;

public partial class ThemeSelectionView : ContentPage
{
    private readonly ThemeSelectionViewModel _viewModel;
	public ThemeSelectionView(ThemeSelectionViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
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

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Dispatcher.Dispatch(async () =>
        {
            PageContainer.TranslationX = -Width;
            PageContainer.Opacity = 0;

            await Task.WhenAll(
                PageContainer.TranslateTo(0, 0, 300, Easing.CubicOut),
                PageContainer.FadeTo(1, 300, Easing.CubicOut)
            );
        });
    }
    protected override bool OnBackButtonPressed() => true;
}