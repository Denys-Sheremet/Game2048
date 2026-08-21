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

        try
        {
            await this.TranslateToAsync(0, 0, duration, Easing.CubicOut);
        }
        finally
        {
            this.TranslationX = 0;
            this.TranslationY = 0;

            DumpState("[MoveToAsync]");
        }
    }

    public async Task AppearAsync(uint duration = 90)
    {
        this.Scale = 0;
        this.Opacity = 0;

        try
        {
            await Task.WhenAll(
                this.ScaleToAsync(1.0, duration, Easing.CubicOut),
                this.FadeToAsync(1.0, duration, Easing.CubicOut)
            );
        }
        finally
        {
            this.Scale = 1.0;
            this.Opacity = 1.0;
        }
        
    }

    public async Task PopAsync(uint duration = 120)
    {
        uint halfDuration = duration / 2;

        try
        {
            await this.ScaleToAsync(1.1, halfDuration, Easing.CubicOut);
            await this.ScaleToAsync(1.0, halfDuration, Easing.CubicIn);
        }
        finally
        {
            this.Scale = 1.0;
        }
        
    }

    public async Task DisappearAsync(uint duration = 90)
    {
        try
        {
            await Task.WhenAll(
                this.FadeToAsync(0.0, duration, Easing.CubicIn),
                this.ScaleToAsync(0.75, duration, Easing.CubicIn)
            );
        }
        finally
        {
            this.Opacity = 0.0;
            this.Scale = 0.75;
        }
    }

    public async Task RespawnToAsync(double toX, double toY, uint duration = 110)
    {
        try
        {
            await Task.WhenAll(
                this.ScaleToAsync(1, duration, Easing.CubicOut),
                this.FadeToAsync(1, duration, Easing.CubicOut),
                this.MoveToAsync(toX, toY, duration)
            );
        }
        finally
        {
            this.Scale = 1.0;
            this.Opacity = 1.0;

            DumpState("[RespawnToAsync]");
        }
        
    }

    //debug
    public void DumpState(string reason)
    {
        var bounds = AbsoluteLayout.GetLayoutBounds(this);

        // Пытаемся безопасно достать ID из привязанной модели
        // Замени `dynamic`, если знаешь точный тип, например (BindingContext as TileVM)
        string idStr = "NULL";
        if (BindingContext != null)
        {
            idStr = (BindingContext as dynamic).Id.ToString();
        }

        System.Diagnostics.Debug.WriteLine(
            $"[TileView] {reason} | " +
            $"ID={idStr} | " +
            $"Parent={Parent?.GetType().Name ?? "NULL"} | " +
            $"Visible={IsVisible} | " +
            $"Opacity={Opacity:F2} | " +
            $"Scale={Scale:F2} | " +
            $"Translation=({TranslationX:F2},{TranslationY:F2}) | " +
            $"Bounds=({bounds.X:F0},{bounds.Y:F0})");
    }
}