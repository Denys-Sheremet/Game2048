
namespace Game2048.Maui.Views.Pages;

public partial class HowToView : ContentPage
{
	public HowToView()
	{
		InitializeComponent();

        Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        PageContainer.Opacity = 0;
        PageContainer.IsVisible = true;
        PageContainer.TranslationX = 200;

        await Task.Yield();

        await Task.WhenAll
            (
                PageContainer.FadeToAsync(1, 300, Easing.CubicOut),
                PageContainer.TranslateToAsync(0, 0, 300, Easing.CubicOut)
            );
    }

    private async Task AnimatePageDisappearingAsync(double translationX)
    {
        await Task.WhenAll
            (
                PageContainer.TranslateToAsync(translationX, 0, 300, Easing.CubicIn),
                PageContainer.FadeToAsync(0, 300, Easing.CubicIn)
            );
    }

    private async Task GoBackAsync()
    {
        await AnimatePageDisappearingAsync(200);

        await Shell.Current.GoToAsync("..", false);
    }

    public async void OnBack(object sender, EventArgs e)
    {
        await GoBackAsync();
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