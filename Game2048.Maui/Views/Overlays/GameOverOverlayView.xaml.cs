namespace Game2048.Maui.Views.Overlays;

public partial class GameOverOverlayView : ContentView
{
    public event EventHandler? GoToMenuRequested;

    public GameOverOverlayView()
	{
		InitializeComponent();
	}

    private void OnGoToMenuClicked(object sender, EventArgs e)
    {
        GoToMenuRequested?.Invoke(this, EventArgs.Empty);
    }
}