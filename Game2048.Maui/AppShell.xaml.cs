using Game2048.Maui.Views.Pages;

namespace Game2048.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ThemeSelectionView), typeof(ThemeSelectionView));
            Routing.RegisterRoute(nameof(HowToView), typeof(HowToView));
        }

        public void SetCurrentToLangSelect()
        {
            this.CurrentItem = LangSelect;
        }
    }
}
