using Game2048.Maui.Interfaces;
using Game2048.Maui.Views.Pages;
namespace Game2048.Maui
{
    public partial class App : Application
    {
        private readonly IProfileManager _profileManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRewardManager _rewardManager;

        public App(IProfileManager profileManager, 
                   ISettingsManager settingsManager, 
                   IServiceProvider serviceProvider,
                   IRewardManager rewardManager)
        {
            InitializeComponent();

            _profileManager = profileManager;
            _settingsManager = settingsManager;
            _serviceProvider = serviceProvider;
            _rewardManager = rewardManager;
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
            if (_profileManager.CurrentProfile is not null)
            {
                try
                {
                    _profileManager.SaveCurrentProfileAsync().Wait(1500);
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as needed
                }
            }
        }
    }
}
