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

        Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        await AnimatePageAppearing();
    }

    private async Task AnimatePageAppearing()
    {
        MenuPageContainer.TranslationX = -200;
        MenuPageContainer.Opacity = 0;
        MenuPageContainer.IsVisible = true;

        await Task.Yield();

        await AnimatePageAppearTo();
    }

    private async Task AnimatePageDissapearTo(int transitionX)
    {
        await Task.WhenAll(
            MenuPageContainer.TranslateToAsync(transitionX, 0, 300, Easing.CubicIn),
            MenuPageContainer.FadeToAsync(0.0, 300, Easing.CubicIn)
        );
    }
    private async Task AnimatePageAppearTo()
    {
        await Task.WhenAll(
            MenuPageContainer.TranslateToAsync(0, 0, 300, Easing.CubicOut),
            MenuPageContainer.FadeToAsync(1.0, 300, Easing.CubicOut)
        );
    }

    private async void OnStartClassicGame(object sender, EventArgs e)
	{
        await AnimatePageDissapearTo(200);

        await Task.WhenAll(
            Shell.Current.GoToAsync("///GamePage", false)
        ); 
    }

    private async void OnGoToAchievements(object sender, EventArgs e)
    {
        await AnimatePageDissapearTo(-200);

        await Task.WhenAll(
            Shell.Current.GoToAsync("///AchievementsPage", false)
        );
    }

    private async void OnGoToGameModes(object sender, EventArgs e)
    {
        await AnimatePageDissapearTo(200);

        await Task.WhenAll(
            Shell.Current.GoToAsync("///GameModesPage", false)
        );
    }

    private async void OnGoToSettings(object sender, EventArgs e)
    {
        await AnimatePageDissapearTo(-200);

        await Task.WhenAll(
            Shell.Current.GoToAsync("///SettingsPage", false)
        );
    }

    private async void OnGoToHowTo(object sender, EventArgs e)
    {
        await AnimatePageDissapearTo(-200);

        await Task.WhenAll(
            Shell.Current.GoToAsync("///HowToPage", false)
        );
    }

    protected override bool OnBackButtonPressed() => true; //maybe quit overlay
}