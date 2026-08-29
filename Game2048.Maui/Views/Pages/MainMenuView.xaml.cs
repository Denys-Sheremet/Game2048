using Game2048.Maui.ViewModels;

namespace Game2048.Maui.Views.Pages;

public partial class MainMenuView : ContentPage
{
	private readonly MainMenuViewModel _viewModel;

    private const double BaseClusterWidth = 290.0;
    private const double BaseClusterHeight = 460.0;
    private const double ReservedVerticalSpace = 310.0;
    private const double HorizontalPadding = 48.0;

    private bool _isPageLoaded = false;
    public MainMenuView(MainMenuViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
		BindingContext = _viewModel;

        Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        _isPageLoaded = true;

        await AnimatePageAppearingAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.PlayRequested += OnStartGame;
        _viewModel.GameModesRequested += OnGoToGameModes;
        _viewModel.AchievementsRequested += OnGoToAchievements;
        _viewModel.ThemesRequested += OnGoToThemes;
        _viewModel.ProfileRequested += OnGoToProfile;
        _viewModel.HowToPlayRequested += OnGoToHowTo;
        _viewModel.SettingsRequested += OnGoToSettings;

        if (_isPageLoaded) 
        {
            Dispatcher.Dispatch(async () =>
            {
                await AnimatePageAppearingAsync();
            });
        }
        
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _viewModel.PlayRequested -= OnStartGame;
        _viewModel.GameModesRequested -= OnGoToGameModes;
        _viewModel.AchievementsRequested -= OnGoToAchievements;
        _viewModel.ThemesRequested -= OnGoToThemes;
        _viewModel.ProfileRequested -= OnGoToProfile;
        _viewModel.HowToPlayRequested -= OnGoToHowTo;
        _viewModel.SettingsRequested -= OnGoToSettings;
    }

    private void OnPageContainerSizeChanged(object? sender, EventArgs e)
    {
        if (MenuPageContainer.Width <= 0 || MenuPageContainer.Height <= 0 || DiamondCluster is null)
            return;

        double availableWidth = MenuPageContainer.Width - HorizontalPadding;
        double availableHeight = MenuPageContainer.Height - ReservedVerticalSpace;

        if (availableWidth <= 0 || availableHeight <= 0)
            return;

        double scaleX = availableWidth / BaseClusterWidth;
        double scaleY = availableHeight / BaseClusterHeight;

        double targetScale = Math.Min(scaleX, scaleY) * 0.86;

        DiamondCluster.Scale = Math.Clamp(targetScale, 0.5, 2.0);
    }

    private async Task AnimatePageDissapearTo(int transitionX)
    {
        await Task.WhenAll(
            MenuPageContainer.TranslateToAsync(transitionX, 0, 300, Easing.CubicIn),
            MenuPageContainer.FadeToAsync(0.0, 300, Easing.CubicIn)
        );
    }
    private async Task AnimatePageAppearingAsync()
    {
        MenuPageContainer.TranslationX = -200;
        MenuPageContainer.Opacity = 0;
        MenuPageContainer.IsVisible = true;

        await Task.Yield();

        await Task.WhenAll(
            MenuPageContainer.TranslateToAsync(0, 0, 300, Easing.CubicOut),
            MenuPageContainer.FadeToAsync(1.0, 300, Easing.CubicOut)
        );
    }

    private async void OnStartGame()
	{
        await AnimatePageDissapearTo(200);

        await Task.WhenAll(
            Shell.Current.GoToAsync("///GamePage", false)
        ); 
    }

    private async void OnGoToAchievements()
    {
        await AnimatePageDissapearTo(-200);

        await Task.WhenAll(
            Shell.Current.GoToAsync("///AchievementsPage", false)
        );
    }

    private async void OnGoToGameModes()
    {
        await AnimatePageDissapearTo(200);

        await Task.WhenAll(
            Shell.Current.GoToAsync("///GameModesPage", false)
        );
    }

    private async void OnGoToSettings()
    {
        await AnimatePageDissapearTo(-200);

        await Task.WhenAll(
            Shell.Current.GoToAsync("///SettingsPage", false)
        );
    }

    private async void OnGoToHowTo()
    {
        await AnimatePageDissapearTo(-200);

        if (Shell.Current.CurrentPage is MainMenuView mainMenu)
        {
            await Shell.Current.GoToAsync(nameof(HowToView), false); 
        }
    }

    private async void OnGoToThemes()
    {
        await AnimatePageDissapearTo(200);

        if (Shell.Current.CurrentPage is MainMenuView mainMenu)
        {
            await Shell.Current.GoToAsync(nameof(ThemeSelectionView), false);
        }
    }

    private async void OnGoToProfile()
    {
        await AnimatePageDissapearTo(-200);

        if (Shell.Current.CurrentPage is MainMenuView mainMenu)
        {
            await Shell.Current.GoToAsync(nameof(ProfilePageView), false);
        }
    }

    protected override bool OnBackButtonPressed() => true; //maybe quit overlay
}