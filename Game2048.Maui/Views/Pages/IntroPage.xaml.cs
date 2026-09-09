using Game2048.Maui.Interfaces;
using Game2048.Maui.Constants;

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

        Task initProfileTask = InitializeProfileAsync();

        await Task.WhenAll(
            IntroLogoImage.FadeToAsync(1, 900, Easing.CubicOut),
            IntroLogoImage.ScaleToAsync(1, 900, Easing.CubicOut)
        );

        await initProfileTask;

        await IntroLogoImage.FadeToAsync(0, 400, Easing.CubicOut);
        

        await Shell.Current.GoToAsync("///MainMenuPage");
    }

    private async Task InitializeProfileAsync()
    {
        bool isLoaded = await _profileManager.TryLoadProfileFromSave();
        if (!isLoaded)
        {
            _profileManager.NewProfile();
            await _profileManager.SaveCurrentProfileAsync();
        }
    }

    protected override bool OnBackButtonPressed() => true;
}