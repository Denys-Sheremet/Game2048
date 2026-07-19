using Game2048.Maui.ViewModels;

namespace Game2048.Maui.Views.Pages;

public partial class GameModesView : ContentPage
{
    private readonly GameModesViewModel _viewModel;
	public GameModesView(GameModesViewModel viewModel)
	{
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        viewModel.OnReadyToPlay += OnStartGame;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Dispatcher.Dispatch(async () =>
        {
            GameModesPageContainer.TranslationX = Width;
            GameModesPageContainer.Opacity = 0;

            await Task.WhenAll(
                GameModesPageContainer.TranslateTo(0, 0, 300, Easing.CubicOut),
                GameModesPageContainer.FadeTo(1, 300, Easing.CubicOut)
            );
        });
    }

    private async void OnBackToMenu(object sender, EventArgs e)
	{
		await Task.WhenAll
			(
                GameModesPageContainer.TranslateTo(Width, 0, 250, Easing.CubicIn),
                GameModesPageContainer.FadeTo(0, 250, Easing.Linear)
            );
		await Task.WhenAll
			(
                Shell.Current.GoToAsync("///MainMenuPage", false)
            );
	}

    private async void OnStartGame()
    {
        await Task.WhenAll
            (
                GameModesPageContainer.TranslateTo(-Width, 0, 250, Easing.CubicIn),
                GameModesPageContainer.FadeTo(0, 250, Easing.Linear)
            );
        await Task.WhenAll
            (
                Shell.Current.GoToAsync("///GamePage", false)
            );
    }

    protected override bool OnBackButtonPressed() => true;
}