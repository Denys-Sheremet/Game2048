using Game2048.Maui.ViewModels;

namespace Game2048.Maui.Views;

public partial class MainMenuView : ContentPage
{
	private readonly MainMenuViewModel _mainMenuViewModel;
	public MainMenuView(MainMenuViewModel viewModel)
	{
		InitializeComponent();
        _mainMenuViewModel = viewModel;
		BindingContext = _mainMenuViewModel;
    }
    private async void OnButtonClicked(object sender, EventArgs e)
    {
        var button = (View)sender;

        await button.ScaleTo(0.85, 50, Easing.CubicIn);
        await button.ScaleTo(1, 100, Easing.SpringOut);
    }
}