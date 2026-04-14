using Game2048.Maui.ViewModels;

namespace Game2048.Maui.Views;

public partial class MainMenuView : ContentPage
{
	private readonly MainMenuViewModel _mainMenuViewModel;
	public MainMenuView()
	{
		InitializeComponent();
        _mainMenuViewModel = new MainMenuViewModel();
		BindingContext = _mainMenuViewModel;
    }
}