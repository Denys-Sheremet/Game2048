using Game2048.Maui.Interfaces;
using Game2048.Maui.Services;

namespace Game2048.Maui.Views.Pages;

public partial class IntroPage : ContentPage
{
    private readonly IProfileManager _profileManager;

    public IntroPage(IProfileManager profileManager)
	{
		InitializeComponent();
        _profileManager = profileManager;
	}

    protected override async void OnAppearing()
	{
		base.OnAppearing();

        IntroLogoImage.Opacity = 0;
        IntroLogoImage.Scale = 0.5;
        IntroLogoImage.WidthRequest = LayoutConstants.GetLogoWidth(400);

        var initProfileTask = InitializeProfileAsync();

        await Task.WhenAll(
            IntroLogoImage.FadeTo(1, 900, Easing.CubicOut),
            IntroLogoImage.ScaleTo(1, 900, Easing.CubicOut)
        );

        await initProfileTask;

        await Task.WhenAll(
            IntroLogoImage.FadeTo(0, 400, Easing.CubicOut)
        );

        await Shell.Current.GoToAsync("///MainMenuPage");
    }

    private async Task InitializeProfileAsync()
    {
        bool isLoaded = await _profileManager.TryLoadProfileFromSave();
        if (!isLoaded)
        {
            _profileManager.NewProfile();
        }
    }
}