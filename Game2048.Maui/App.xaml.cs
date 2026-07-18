using Game2048.Maui.Interfaces;
namespace Game2048.Maui
{
    public partial class App : Application
    {
        private readonly IProfileManager _profileManager;

        public App(IProfileManager profileManager)
        {
            InitializeComponent();

            _profileManager = profileManager;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
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
