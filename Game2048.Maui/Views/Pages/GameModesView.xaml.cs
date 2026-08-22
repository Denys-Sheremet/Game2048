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
        _viewModel.OnReadyToPlay += OnStartGame;

        Loaded += OnPageLoaded;
	}

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        await AnimatePageAppearingAsync(-200);
    }

    private async Task AnimatePageAppearingAsync(double translationX)
    {
        await Task.Yield();

        GameModesPageContainer.TranslationX = translationX;
        GameModesPageContainer.Opacity = 0;
        GameModesPageContainer.IsVisible = true;

        await Task.WhenAll(
            GameModesPageContainer.TranslateToAsync(0, 0, 300, Easing.CubicOut),
            GameModesPageContainer.FadeToAsync(1, 300, Easing.CubicOut)
        );
    }

    private async Task AnimatePageDisappearingAsync(double translationX)
    {
        await Task.WhenAll(
            GameModesPageContainer.TranslateToAsync(translationX, 0, 300, Easing.CubicIn),
            GameModesPageContainer.FadeToAsync(0, 300, Easing.CubicIn)
        );
    }

    private async Task GoBackToMenuAsync()
    {
        await AnimatePageDisappearingAsync(-200);

        await Task.WhenAll
            (
                Shell.Current.GoToAsync("///MainMenuPage", false)
            );
    }

    private async void OnBackToMenu(object sender, EventArgs e)
	{
        await GoBackToMenuAsync();
    }

    private async void OnStartGame()
    {
        await AnimatePageDisappearingAsync(-200);

        await Task.WhenAll
            (
                Shell.Current.GoToAsync("///GamePage", false)
            );
    }

    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            await GoBackToMenuAsync();
        });
        return true;
    }
}