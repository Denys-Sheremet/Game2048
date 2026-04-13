namespace Game2048.Maui.Views;

public partial class TileView : Border
{
	public TileView()
	{
		InitializeComponent();
	}

	public async Task MoveToAsync(double toX,  double toY, uint duration = 120)
	{
		var currBounds = AbsoluteLayout.GetLayoutBounds(this);

        double deltaX = currBounds.X - toX;
        double deltaY = currBounds.Y - toY;

		AbsoluteLayout.SetLayoutBounds(this, new Rect(toX, toY, currBounds.Width, currBounds.Height));

        this.TranslationX = deltaX;
        this.TranslationY = deltaY;

        await this.TranslateTo(0, 0, duration, Easing.CubicOut);
    }

    public async Task AppearAsync(uint duration = 200)
    {
        this.Scale = 0;
        this.Opacity = 0;
        await Task.WhenAll(
            this.ScaleTo(1.0, duration, Easing.SpringOut),
            this.FadeTo(1.0, duration)
        );
    }

    public async Task PopAsync(uint duration = 100)
    {
        await this.ScaleTo(1.2, duration / 2, Easing.CubicIn);
        await this.ScaleTo(1.0, duration / 2, Easing.CubicOut);
    }

    public async Task DisappearAsync(uint duration = 80)
    {
        await Task.WhenAll(
            this.ScaleTo(0.0, duration, Easing.CubicIn),
            this.FadeTo(0.0, duration, Easing.CubicIn)
        );
    }
}