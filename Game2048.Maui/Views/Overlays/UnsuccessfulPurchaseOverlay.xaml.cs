namespace Game2048.Maui.Views.Overlays;

public partial class UnsuccessfulPurchaseOverlay : ContentView
{
	public event Action? HideUnsuccessfulPurchaseOverlayRequested;
    public UnsuccessfulPurchaseOverlay()
	{
		InitializeComponent();
	}

	private void OnCloseUnsuccessfulPurchaseOverlay(object sender, EventArgs e)
    {
        HideUnsuccessfulPurchaseOverlayRequested?.Invoke();
    }
}