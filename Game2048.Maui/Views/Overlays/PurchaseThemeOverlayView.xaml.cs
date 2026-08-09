using Game2048.Maui.ViewModels.Items;

namespace Game2048.Maui.Views.Overlays;

public partial class PurchaseThemeOverlayView : ContentView
{
    public PurchaseThemeOverlayView()
	{
		InitializeComponent();
	}

	public async void OnClosePurchaseThemeOverlay(object sender, EventArgs e)
    {
        //invoke the close overlay event in main view 
    }
}