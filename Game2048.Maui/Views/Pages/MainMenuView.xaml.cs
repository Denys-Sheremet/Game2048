using Game2048.Maui.ViewModels;

namespace Game2048.Maui.Views.Pages;

public partial class MainMenuView : ContentPage
{
	private readonly MainMenuViewModel _mainMenuViewModel;
	public MainMenuView(MainMenuViewModel viewModel)
	{
		InitializeComponent();
        _mainMenuViewModel = viewModel;
		BindingContext = _mainMenuViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Dispatcher.Dispatch(async () =>
        {
            MenuPageContainer.TranslationX = -Width;
            MenuPageContainer.Opacity = 0;

            await Task.WhenAll(
                MenuPageContainer.TranslateTo(0, 0, 300, Easing.SpringOut),
                MenuPageContainer.FadeTo(1, 300, Easing.CubicOut)
            );
        });
    }

	private async void OnStartClassicGame(object sender, EventArgs e)
	{
        await Task.WhenAll(
            MenuPageContainer.TranslateTo(-Width, 0, 250, Easing.CubicIn),
            MenuPageContainer.FadeTo(0, 250, Easing.Linear)
        );

        await Task.WhenAll(
            Shell.Current.GoToAsync("///GamePage", false)
        ); 
    }

    private async void OnGoToAchievements(object sender, EventArgs e)
    {
        await Task.WhenAll(
            MenuPageContainer.TranslateTo(0, Height, 250, Easing.CubicIn),
            MenuPageContainer.FadeTo(0, 250, Easing.Linear)
        );

        await Task.WhenAll(
            Shell.Current.GoToAsync("///AchievementsPage", false)
        );
    }

    private async void OnGoToGameModes(object sender, EventArgs e)
    {
        await Task.WhenAll(
            MenuPageContainer.TranslateTo(-Width, 0, 250, Easing.CubicIn),
            MenuPageContainer.FadeTo(0, 250, Easing.Linear)
        );

        await Task.WhenAll(
            Shell.Current.GoToAsync("///GameModesPage", false)
        );
    }

    private async void OnGoToSettings(object sender, EventArgs e)
    {
        await Task.WhenAll(
            MenuPageContainer.TranslateTo(Width, 0, 250, Easing.CubicIn),
            MenuPageContainer.FadeTo(0, 250, Easing.Linear)
        );

        await Task.WhenAll(
            Shell.Current.GoToAsync("///SettingsPage", false)
        );
    }

    private async void OnGoToHowTo(object sender, EventArgs e)
    {
        await Task.WhenAll(
            MenuPageContainer.TranslateTo(0, -Height, 250, Easing.CubicIn),
            MenuPageContainer.FadeTo(0, 250, Easing.Linear)
        );

        await Task.WhenAll(
            Shell.Current.GoToAsync("///HowToPage", false)
        );
    }
}