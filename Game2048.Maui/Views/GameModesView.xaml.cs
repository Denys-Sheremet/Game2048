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

}