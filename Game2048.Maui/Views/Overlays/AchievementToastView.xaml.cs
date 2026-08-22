using Game2048.Maui.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Game2048.Maui.Views.Overlays;

public partial class AchievementToastView : ContentView
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    private readonly ILogger<AchievementToastView> _logger;

    public AchievementToastView()
    {
        InitializeComponent();
        _logger = IPlatformApplication.Current?.Services.GetService<ILogger<AchievementToastView>>()
                  ?? NullLogger<AchievementToastView>.Instance;
    }
    public AchievementToastView(ILogger<AchievementToastView>? logger = null)
	{
		InitializeComponent();
        _logger = logger ?? NullLogger<AchievementToastView>.Instance;
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
        catch (OperationCanceledException) 
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to display achievement toast view");
        }
        finally
        {
            ToastContainer.IsVisible = false;
            _semaphore.Release();
        }
    }
}