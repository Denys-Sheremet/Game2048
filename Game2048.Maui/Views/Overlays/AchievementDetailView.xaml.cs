namespace Game2048.Maui.Views.Overlays;

public partial class AchievementDetailView : ContentView
{
	public AchievementDetailView()
	{
		InitializeComponent();
	}

	public async void OnCloseDetail(object sender, EventArgs e)
	{
		await CloseAsync();
    }

	public async Task CloseAsync()
	{
		if (this.IsVisible)
		{
            await Dispatcher.DispatchAsync(async () =>
            {
                await this.ScaleToAsync(1.1, 150, Easing.CubicIn);
                await Task.WhenAll(
                    this.FadeToAsync(0, 250, Easing.CubicIn),
                    this.ScaleToAsync(0.7, 250, Easing.CubicIn)
                );
                this.IsVisible = false;
            });
        }
	}

	public async Task ShowAsync(string title, string desc, string imageName)
	{
        if (this.IsVisible) await CloseAsync();

        this.DetailTitleLabel.Text = title;
        this.DetailDescLabel.Text = desc;
        this.DetailImage.Source = imageName;

        await Dispatcher.DispatchAsync(async () =>
		{
            this.IsVisible = true;
			this.Scale = 0.0;
			this.Opacity = 0.0;
            await Task.WhenAll(
                this.FadeToAsync(1, 250, Easing.CubicIn),
                this.ScaleToAsync(1.1, 250, Easing.CubicIn)
            );
            await this.ScaleToAsync(1.0, 150, Easing.CubicIn);
        });
    }
}