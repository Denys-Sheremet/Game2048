using CommunityToolkit.Mvvm.Input;
using Game2048.Core.Enums;
using Game2048.Maui.Enums;
using Game2048.Core.Models;
using Game2048.Maui.Interfaces;
using Game2048.Core.Interfaces;
using System.Windows.Input;

namespace Game2048.Maui.ViewModels;

public partial class MainMenuViewModel : BindableObject
{
    private readonly GameConfig _gameConfig;
    private readonly ISaveService _saveService;
    private readonly IProfileManager _profileManager;

    public MainMenuViewModel(GameConfig gameConfig, ISaveService saveService, IProfileManager profileManager)
    {
        _gameConfig = gameConfig;
        _saveService = saveService;
        _profileManager = profileManager;
    }
}
