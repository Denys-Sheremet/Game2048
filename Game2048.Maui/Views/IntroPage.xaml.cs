using Game2048.Maui.Services;

namespace Game2048.Maui.Views;

public partial class IntroPage : ContentPage
{
	public IntroPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

        IntroLogoImage.Opacity = 0;
        IntroLogoImage.Scale = 0.5;
        IntroLogoImage.WidthRequest = LayoutConstants.GetLogoWidth(400);

        await Task.WhenAll(
            IntroLogoImage.FadeTo(1, 900, Easing.CubicOut),
            IntroLogoImage.ScaleTo(1, 900, Easing.CubicOut)
        );

        await Task.WhenAll(
            IntroLogoImage.FadeTo(0, 400, Easing.CubicOut)
        );

        await Shell.Current.GoToAsync("///MainMenuPage");
    }
}