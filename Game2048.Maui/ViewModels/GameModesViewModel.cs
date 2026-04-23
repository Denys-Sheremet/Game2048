using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.ViewModels;

public partial class GameModesViewModel : BindableObject
{
    public IAsyncRelayCommand BackToMenuCommand { get; private set; }

    public GameModesViewModel() 
    {
        BackToMenuCommand = new AsyncRelayCommand(OnGoToMenu);
    }

    private async Task OnGoToMenu()
    {
        await Shell.Current.GoToAsync("///MainMenuPage");
    }
}
