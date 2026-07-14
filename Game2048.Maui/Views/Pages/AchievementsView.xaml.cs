using Game2048.Maui.ViewModels;
using Game2048.Maui.Views.Components;

namespace Game2048.Maui.Views.Pages;

public partial class AchievementsView : ContentPage
{
    private int _cardCounter = 0;
    private bool _isInitialLoad = false;
    public AchievementsView(AchievementsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _cardCounter = 0;
        _isInitialLoad = true;

        await Task.Delay(150);

        _isInitialLoad = false;
    }

    private async void OnAchievementTileLoaded(object sender, EventArgs e)
    {
        if (sender is ContentView tile)
        {
            tile.Opacity = 0;
            tile.Scale = 0.5;

            int delay = 0;

            if (_isInitialLoad)
            {
                delay = _cardCounter * 80;
                _cardCounter++;
            }

            if (delay > 0)
            {
                await Task.Delay(delay);
            }

            await Task.WhenAll(
                tile.FadeTo(1, 350, Easing.CubicOut),
                tile.ScaleTo(1, 350, Easing.SpringOut)
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

            await clickedView.ScaleTo(0.92, 100, Easing.CubicOut);

            await clickedView.ScaleTo(1.0, 200, Easing.SpringOut);
        }
    }

    private async void OnBackToMenu(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///MainMenuPage", false);
    }
}