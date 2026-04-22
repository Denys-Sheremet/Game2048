using System.Windows.Input;

namespace Game2048.Maui.ViewModels;

public class MainMenuViewModel : BindableObject
{
    public ICommand StartGameCommand { get; }

    public MainMenuViewModel()
    {
        StartGameCommand = new Command(async () => await OnStartGame());
    }

    private async Task OnStartGame()
    {
        await Shell.Current.GoToAsync("///ClassicGamePage");
    }
}
