using Game2048.Maui.ViewModels;
using Game2048.Maui.Views.Components;

namespace Game2048.Maui.Views.Pages;

public partial class AchievementsView : ContentPage
{
    private bool _isInitialLoad = false;

    private readonly AchievementsViewModel _viewModel;
    public AchievementsView(AchievementsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        _viewModel.OnAchievementSelected += async (title, desc, imgName) =>
        {
            await ShowDetails(title, desc, imgName);
        };

        Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        await AnimatePageAppearingAsync();

        _isInitialLoad = true;

        await _viewModel.InitializeDataAsync();
        await Task.Delay(800);

        _isInitialLoad = false;
    }

    private async void OnProgressBarTapped(object? sender, EventArgs e)
    {
        await ProgressBarContainer.ScaleToAsync(0.95, 100, Easing.CubicOut);
        await ProgressBarContainer.ScaleToAsync(1.05, 100, Easing.CubicOut);
        await ProgressBarContainer.ScaleToAsync(1.0, 100, Easing.CubicOut);
    }

    private async Task AnimatePageAppearingAsync()
    {
        AchievementsPageContainer.TranslationX = 200;
        AchievementsPageContainer.Opacity = 0;
        AchievementsPageContainer.IsVisible = true;

        await Task.WhenAll(
            AchievementsPageContainer.TranslateToAsync(0, 0, 300, Easing.CubicOut),
            AchievementsPageContainer.FadeToAsync(1, 300, Easing.CubicOut)
        );
    }

    private async Task AnimatePageDisappearingAsync(double translationX)
    {
        await Task.WhenAll(
            AchievementsPageContainer.TranslateToAsync(translationX, 0, 300, Easing.CubicIn),
            AchievementsPageContainer.FadeToAsync(0, 300, Easing.CubicIn)
        );
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }

    private async void OnAchievementTileLoaded(object sender, EventArgs e)
    {
        if (sender is ContentView tile && tile.BindingContext is AchievementData data)
        {
            tile.Opacity = 0;
            tile.Scale = 0.5;

            int delay = 0;

            if (_isInitialLoad)
            {
                int index = _viewModel.Achievements.IndexOf(data);
                if (index >= 0)
                {
                    int visualIndex = Math.Min(index, 12);
                    delay = visualIndex * 80;
                }
            }

            if (delay > 0)
            {
                await Task.Delay(delay);
            }

            await Task.WhenAll(
                tile.FadeToAsync(1, 350, Easing.CubicOut),
                tile.ScaleToAsync(1, 350, Easing.SpringOut)
            );
        }
    }

    private void OnPageSizeChanged(object sender, EventArgs e)
    {
        if (this.Width > 0)
        {
            double itemWidth = 125.0;
            int columns = (int)(this.Width / itemWidth);
            AchievementsLayout.Span = Math.Max(3, columns);
        }
    }

    private async void OnCardTapped(object sender, TappedEventArgs e)
    {
        if (sender is View clickedView)
        {
            Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(clickedView);

            await clickedView.ScaleToAsync(0.92, 100, Easing.CubicOut);
            await clickedView.ScaleToAsync(1.0, 200, Easing.SpringOut);

            if (clickedView.BindingContext is AchievementData tappedAchievement)
            {
                _viewModel.OpenAchievementCommand.Execute(tappedAchievement);
            }
        }
    }

    private async Task GoBackToMenuAsync()
    {
        await AnimatePageDisappearingAsync(200);

        await Shell.Current.GoToAsync("///MainMenuPage", false);
    }

    private async void OnBackToMenu(object sender, EventArgs e)
    {
        await GoBackToMenuAsync();
    }

    public async Task ShowDetails(string title, string desc, string imageName)
    {
        await DetailOverlay.ShowAsync(title, desc, imageName);
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