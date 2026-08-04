using Game2048.Maui.ViewModels.Items;

namespace Game2048.Maui.Views.Overlays;

public partial class ThemePreviewOverlayView : ContentView
{
	public event Action? HideThemePreviewRequested;
    public ThemePreviewOverlayView()
	{
		InitializeComponent();
    }

	public void OnHideThemePreview(object sender, EventArgs e)
	{
        HideThemePreviewRequested?.Invoke();
    }
}