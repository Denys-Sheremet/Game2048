using Game2048.Maui.ViewModels;

namespace Game2048.Maui.Views.Components;

public partial class ThemeItemView : ContentView
{
	public ThemeItemView()
	{
		InitializeComponent();
	}

	public async void OnBtnTapped(object sender, EventArgs e)
	{
		if (sender is Border br)
		{
			await br.ScaleTo(0.95, 100, Easing.CubicIn);
			await br.ScaleTo(1.03, 150, Easing.CubicIn);
			await br.ScaleTo(1.0, 50, Easing.CubicIn);
		}
	}
}