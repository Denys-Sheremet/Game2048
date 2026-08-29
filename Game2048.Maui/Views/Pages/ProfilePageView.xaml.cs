using Game2048.Maui.ViewModels;

namespace Game2048.Maui.Views.Pages;

public partial class ProfilePageView : ContentPage
{
    private readonly ProfilePageViewModel _viewModel;
	public ProfilePageView(ProfilePageViewModel viewModel)
	{
		InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
	}

    public async void OnBack(object sender, EventArgs e)
    {
        await GoBackAsync();
    }

    public async void OnCardTapped(object sender, EventArgs e)
    {
        if (sender is Border card)
        {
            await card.ScaleToAsync(0.95, 100, Easing.CubicOut);
            await card.ScaleToAsync(1.0, 100, Easing.CubicOut);
        }
    }

    private async Task AnimatePageDissapearTo(int transitionX)
    {
        await Task.WhenAll(
            PageContainer.TranslateToAsync(transitionX, 0, 300, Easing.CubicIn),
            PageContainer.FadeToAsync(0.0, 300, Easing.CubicIn)
        );
    }

    private async Task GoBackAsync()
    {
        await AnimatePageDissapearTo(200);
        await Shell.Current.GoToAsync("..", false);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await AnimatePageAppearingAsync();
    }

    private async Task AnimatePageAppearingAsync()
    {
        PageContainer.TranslationX = 200;
        PageContainer.Opacity = 0;
        PageContainer.IsVisible = true;

        await Task.Yield();

        await Task.WhenAll(
            PageContainer.TranslateToAsync(0, 0, 300, Easing.CubicOut),
            PageContainer.FadeToAsync(1.0, 300, Easing.CubicOut)
        );
    }

    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            await GoBackAsync();
        });
        return true;
    }
}