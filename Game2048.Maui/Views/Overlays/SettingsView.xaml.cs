using Game2048.Maui.Views.Pages;

namespace Game2048.Maui.Views.Overlays;

public partial class SettingsView : ContentView
{
	public SettingsView()
	{
		InitializeComponent();
	}

    private async void OnThemesClicked(object sender, EventArgs e)
    {
        if (Shell.Current.CurrentPage is GameView gamePage)
        {
            await gamePage.AnimateAndNavigateToThemesAsync();
        }
    }

    private void OnHowToClicked(object sender, EventArgs e)
    {
        // TODO
    }
}