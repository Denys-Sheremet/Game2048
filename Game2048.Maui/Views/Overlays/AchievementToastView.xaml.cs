namespace Game2048.Maui.Views.Overlays;

public partial class AchievementToastView : ContentView
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public AchievementToastView()
	{
		InitializeComponent();
	}

    public async Task ShowAsync(string title, string description, string imageName)
    {
        await _semaphore.WaitAsync();

        try
        {
            AchievementTitleLabel.Text = title;
            AchievementDescLabel.Text = description;
            AchievementIcon.Source = imageName;

            ToastContainer.Opacity = 0.0;
            ToastContainer.TranslationY = -300;
            ToastContainer.IsVisible = true;


            await Task.WhenAll
                (
                    ToastContainer.FadeToAsync(1.0, 300, Easing.CubicIn),
                    ToastContainer.TranslateToAsync(0, 0, 600, Easing.BounceOut)
                );

            await Task.Delay(3000);

            await Task.WhenAll
                (
                    ToastContainer.FadeToAsync(0.5, 200, Easing.CubicIn),
                    ToastContainer.TranslateToAsync(0, -300, 400, Easing.CubicIn)
                );
            ToastContainer.IsVisible = false;
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Error in AchievementToast: {ex.Message}");
#endif
        }
        finally
        {
            _semaphore.Release();
        }
    }
}