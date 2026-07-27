using Microsoft.Maui.Controls.Compatibility;

namespace Game2048.Maui.Views.Overlays;

public partial class ConfirmationOverlayView : ContentView
{
	public ConfirmationOverlayView()
	{
		InitializeComponent();
	}

	public async void OnCloseConfirmationOverlay(object sender, EventArgs e)
	{
        await Task.WhenAll
            (
                ConfirmationOverlay.ScaleTo(0.5, 150, Easing.CubicIn),
                ConfirmationOverlay.FadeTo(0.0, 150, Easing.CubicIn)
            );
        ConfirmationOverlay.IsVisible = false;
        if (ConfirmationOverlay.Parent is Microsoft.Maui.Controls.Grid ovrContainer)
        {
            ovrContainer.IsVisible = false;
        }
    }
}