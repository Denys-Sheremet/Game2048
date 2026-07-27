using Game2048.Maui.Interfaces;
using Game2048.Maui.Views.Pages;
namespace Game2048.Maui
{
    public partial class App : Application
    {
        private readonly IProfileManager _profileManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IServiceProvider _serviceProvider;

        public App(IProfileManager profileManager, ISettingsManager settingsManager, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _profileManager = profileManager;
            _settingsManager = settingsManager;
            _serviceProvider = serviceProvider;
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
                Task.Run(async () =>
                {
                    await _profileManager.SaveCurrentProfileAsync();
                });
            }
        }
    }
}
