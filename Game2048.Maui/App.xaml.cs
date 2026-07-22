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
            var shell = new AppShell();

            if (!_settingsManager.LoadInitialSettings())
            {
                var langPage = shell.Items.FirstOrDefault(i => i.Route == "LangSelectPage");
                if (langPage is not null)
                {
                    shell.CurrentItem = langPage;
                }
            }

            return new Window(shell);
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
