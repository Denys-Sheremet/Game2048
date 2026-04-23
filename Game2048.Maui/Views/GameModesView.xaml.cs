using Game2048.Maui.ViewModels;

namespace Game2048.Maui.Views;

public partial class GameModesView : ContentPage
{
    private GameModesViewModel _viewModel;
	public GameModesView(GameModesViewModel viewModel)
	{
        _viewModel = viewModel;
        BindingContext = _viewModel;
		InitializeComponent();
	}

    private async void OnButtonClicked(object sender, EventArgs e)
    {
        var button = (View)sender;

        await button.ScaleTo(0.85, 50, Easing.CubicIn);
        await button.ScaleTo(1, 100, Easing.SpringOut);
    }
}