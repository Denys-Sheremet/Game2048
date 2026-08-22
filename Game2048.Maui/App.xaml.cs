using Game2048.Maui.Interfaces;
using Game2048.Maui.Views.Pages;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Game2048.Maui
{
    public partial class App : Application
    {
        private readonly ILogger<App> _logger;
        private readonly IProfileManager _profileManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRewardManager _rewardManager;

        public App(ILogger<App> logger,
                   IProfileManager profileManager, 
                   ISettingsManager settingsManager, 
                   IServiceProvider serviceProvider,
                   IRewardManager rewardManager)
        {
            InitializeComponent();

            _logger = logger;
            _profileManager = profileManager;
            _settingsManager = settingsManager;
            _serviceProvider = serviceProvider;

            //init to sub to other managers and work in background
            _rewardManager = rewardManager;

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                _logger.LogCritical(e.ExceptionObject as Exception, "Unhandled CurrentDomain exception occurred");
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                _logger.LogError(e.Exception, "Unobserved TaskException occurred");
                e.SetObserved();
            };
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var shell = _serviceProvider.GetRequiredService<AppShell>();

            bool isInit = _settingsManager.LoadInitialSettings();

            if (!isInit)
            {
                shell.SetCurrentToLangSelect();
            }

            return new Window(shell);
        }

        public void RestartApp(string initialRoute = "///MainMenuPage")
        {
            if (Windows.Count > 0)
            {
                var newShell = _serviceProvider.GetRequiredService<AppShell>();

                Windows[0].Page = newShell;

                Dispatcher.Dispatch(async () =>
                {
                    await newShell.GoToAsync(initialRoute, false);
                });
            }
        }

        protected override void OnSleep()
        {
            base.OnSleep();

            if (_profileManager.CurrentProfile is null)
            {
                return;
            }
            try
            {
                _profileManager.SaveCurrentProfileSync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save profile synchronously during OnSleep");
            }
        }
    }
}
