using Game2048.Maui.Services;
using Game2048.Maui.Interfaces;

namespace Game2048.Maui.Views;

public partial class IntroPage : ContentPage
{
    private readonly ISaveService _saveService;
    private readonly IProfileManager _profileManager;

    public IntroPage(ISaveService saveService, IProfileManager profileManager)
	{
		InitializeComponent();
        _saveService = saveService;
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
        var profile = await _saveService.LoadProfileAsync();

        if (profile is not null)
        {
            _profileManager.CurrentProfile = profile;
        }
        else
        {
            _profileManager.NewProfile();
        }
    }
}