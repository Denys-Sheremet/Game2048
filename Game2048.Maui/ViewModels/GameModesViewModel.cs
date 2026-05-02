using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Game2048.Core.Enums;
using Game2048.Core.Models;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Models;
using System.Collections.ObjectModel;

namespace Game2048.Maui.ViewModels;

public partial class GameModesViewModel : ObservableObject
{
    public ObservableCollection<GameMode> Modes { get; }

    private readonly GameConfig _gameConfig;
    public IRelayCommand StartGameCommand { get; private set; }

    public event Action? OnReadyToPlay;

    private GameMode? _selectedMode;
    public GameMode? SelectedMode
    {
        get => _selectedMode;
        set 
        {
            if(SetProperty(ref _selectedMode, value))
            {
                UpdateActiveCardState(value!);
            }
        }
    }

    public GameModesViewModel(IGameModeService gameModeService, GameConfig gameConfig) 
    {
        _gameConfig = gameConfig;
        var gameModes = gameModeService.GetAvailableGameModes();
        Modes = new ObservableCollection<GameMode>(gameModes);

        StartGameCommand =  new RelayCommand<GameModeType>(OnStartGameRequested);

        SelectedMode = Modes.FirstOrDefault();
    }

    private void UpdateActiveCardState(GameMode activeMode) 
    {
        foreach (var mode in Modes) 
        {
            mode.IsActive = (mode == activeMode);
        }
    }

    private void OnStartGameRequested(GameModeType modeType)
    {
        _gameConfig.SetConfig(modeType);

        OnReadyToPlay?.Invoke();
    }
}
