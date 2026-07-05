using Game2048.Core.DTOs;
using Game2048.Core.Models;
using Game2048.Core.Enums;
using Game2048.Maui.Interfaces;

namespace Game2048.Maui.Services;

public class StatisticsManager : IStatisticsManager
{
    private readonly IProfileManager _profileManager;

    private bool _gameEnded = false;
    private bool _hasWon = false;
    private int _movesMade = 0;
    private int _undosMade = 0;

    public StatisticsManager(IProfileManager profileManager)
    {
        _profileManager = profileManager;
    }

    public void Moved()
    {
            _movesMade++;
    }

    public void Undone()
    {
            _undosMade++;
    }

    public void GameEnded(bool hasWon = false)
    {
        _gameEnded = true;
        _hasWon = hasWon;
    }

    public void Reset() 
    {         
        _gameEnded = false;
        _hasWon = false;
        _movesMade = 0;
        _undosMade = 0;
    }

    public void Push()
    {
        if (_profileManager.CurrentProfile is null) 
            throw new InvalidOperationException("No current profile found");

        _profileManager.UpdateStatistics(_gameEnded, _hasWon, _movesMade, _undosMade);
        Reset();
    }
}
