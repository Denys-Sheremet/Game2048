using Game2048.Maui.Views.Pages;

namespace Game2048.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ThemeSelectionView), typeof(ThemeSelectionView));
        }

        public void SetCurrentToLangSelect()
        {
            this.CurrentItem = LangSelect;
        }
    }
}
