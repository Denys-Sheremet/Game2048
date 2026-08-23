using Game2048.Maui.Views.Pages;

namespace Game2048.Maui.Views.Overlays;

public partial class SettingsView : ContentView
{
    public event Action? GoToMenuRequested;

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

    private async void OnHowToClicked(object sender, EventArgs e)
    {
        if (Shell.Current.CurrentPage is GameView gamePage)
        {
            await gamePage.AnimateAndNavigateToHowToAsync();
        }
    }

    private void OnMenuClicked(object sender, EventArgs e)
    {
        GoToMenuRequested?.Invoke();
    }
}