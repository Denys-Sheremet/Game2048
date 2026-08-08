using Microsoft.Maui.Controls;

namespace Game2048.Maui.Views.Components;

public partial class TileView : Border
{
	public TileView()
	{
		InitializeComponent();
	}

	public async Task MoveToAsync(double toX,  double toY, uint duration = 110)
	{
		var currBounds = AbsoluteLayout.GetLayoutBounds(this);

        double deltaX = currBounds.X - toX;
        double deltaY = currBounds.Y - toY;

        this.TranslationX = deltaX;
        this.TranslationY = deltaY;

        AbsoluteLayout.SetLayoutBounds(this, new Rect(toX, toY, currBounds.Width, currBounds.Height));

        await this.TranslateToAsync(0, 0, duration, Easing.CubicOut);
    }

    public async Task AppearAsync(uint duration = 90)
    {
        this.Scale = 0;
        this.Opacity = 0;
        await Task.WhenAll(
            this.ScaleToAsync(1.0, duration, Easing.CubicOut),
            this.FadeToAsync(1.0, duration, Easing.CubicOut)
        );
    }

    public async Task PopAsync(uint duration = 120)
    {
        uint halfDuration = duration / 2;
        await this.ScaleToAsync(1.1, halfDuration, Easing.CubicOut);
        await this.ScaleToAsync(1.0, halfDuration, Easing.CubicIn);
    }

    public async Task DisappearAsync(uint duration = 90)
    {
        await Task.WhenAll(
            this.FadeToAsync(0.0, duration, Easing.CubicIn),
            this.ScaleToAsync(0.75, duration, Easing.CubicIn)
        );
    }

    public async Task RespawnToAsync(double toX, double toY, uint duration = 110)
    {
        await Task.WhenAll(
            this.ScaleToAsync(1, duration, Easing.CubicOut),
            this.FadeToAsync(1, duration, Easing.CubicOut),
            this.MoveToAsync(toX, toY, duration)
        );
    }
}