using Game2048.Maui.Interfaces;
namespace Game2048.Maui
{
    public partial class App : Application
    {
        private readonly ISaveService _saveService;
        private readonly IProfileManager _profileManager;

        public App(ISaveService saveService, IProfileManager profileManager)
        {
            InitializeComponent();

            _saveService = saveService;
            _profileManager = profileManager;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnSleep()
        {
            var profileToSave = _profileManager.CurrentProfile;

            if (profileToSave != null)
            {
                await _saveService.SaveProfileAsync(profileToSave);
            }
        }
    }
}
