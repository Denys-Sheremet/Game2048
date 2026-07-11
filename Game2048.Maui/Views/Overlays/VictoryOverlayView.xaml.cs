namespace Game2048.Maui.Views.Overlays;

public partial class VictoryOverlayView : ContentView
{
    public event EventHandler? GoToMenuRequested;

    public VictoryOverlayView()
	{
		InitializeComponent();
	}

    private void OnGoToMenuClicked(object sender, EventArgs e)
    {
        GoToMenuRequested?.Invoke(this, EventArgs.Empty);
    }
}