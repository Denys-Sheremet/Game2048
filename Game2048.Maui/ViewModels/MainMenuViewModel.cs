using CommunityToolkit.Mvvm.Input;
using Game2048.Core.Models;
using System.Windows.Input;

namespace Game2048.Maui.ViewModels;

public partial class MainMenuViewModel : BindableObject
{
    private readonly GameConfig _gameConfig;

    public MainMenuViewModel(GameConfig gameConfig)
    {
        _gameConfig = gameConfig;
    }
}
