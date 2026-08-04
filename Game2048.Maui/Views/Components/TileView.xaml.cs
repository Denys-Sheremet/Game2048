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

        await this.TranslateTo(0, 0, duration, Easing.CubicOut);
    }

    public async Task AppearAsync(uint duration = 90)
    {
        this.Scale = 0;
        this.Opacity = 0;
        await Task.WhenAll(
            this.ScaleTo(1.0, duration, Easing.CubicOut),
            this.FadeTo(1.0, duration, Easing.CubicOut)
        );
    }

    public async Task PopAsync(uint duration = 120)
    {
        uint halfDuration = duration / 2;
        await this.ScaleTo(1.1, halfDuration, Easing.CubicOut);
        await this.ScaleTo(1.0, halfDuration, Easing.CubicIn);
    }

    public async Task DisappearAsync(uint duration = 90)
    {
        await Task.WhenAll(
            this.FadeTo(0.0, duration, Easing.CubicIn),
            this.ScaleTo(0.75, duration, Easing.CubicIn)
        );
    }

    public async Task RespawnToAsync(double toX, double toY, uint duration = 110)
    {
        await Task.WhenAll(
            this.ScaleTo(1, duration, Easing.CubicOut),
            this.FadeTo(1, duration, Easing.CubicOut),
            this.MoveToAsync(toX, toY, duration)
        );
    }
}