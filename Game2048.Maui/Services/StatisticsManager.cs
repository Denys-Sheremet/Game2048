using Game2048.Core.DTOs;
using Game2048.Core.Models;
using Game2048.Core.Enums;
using Game2048.Maui.Interfaces;

namespace Game2048.Maui.Services;

public class StatisticsManager : IStatisticsManager
{
    private readonly IProfileManager _profileManager;

    public bool GameOver { get; private set; } = false;
    public bool HasWon { get; private set; } = false;
    public int MovesMade { get; private set; } = 0;
    public int UndosMade { get; private set; } = 0;
    public GameModeType CurrentGameMode { get; private set; } = GameModeType.Classic;

    // This property tracks the total number of tiles merged during the game session
    // Not saved in profile
    public int TilesMerged { get; private set; } = 0;

    //Easter egg =)
    public int UselessUndoClicked { get; private set; } = 0;

    public StatisticsManager GetStatisticsManager()
    {
        return this;
    }
    public StatisticsManager(IProfileManager profileManager)
    {
        _profileManager = profileManager;
    }
    public void SetCurrentGameMode(GameModeType gameMode)
    {
        CurrentGameMode = gameMode;
    }
    public void Merged(int count)
    {
        TilesMerged += count;
    }

    public void Respawned(int count)
    {
        TilesMerged -= count;
    }

    public void Moved()
    {
        MovesMade++;
    }

    public void Undone()
    {
        UndosMade++;
    }

    public void UselessUndone()
    {
        UselessUndoClicked++;
    }

    public void GameEnded(bool hasWon = false)
    {
        GameOver = true;
        HasWon = hasWon;
    }

    public void Reset() 
    {
        GameOver = false;
        HasWon = false;
        MovesMade = 0;
        UndosMade = 0;
        TilesMerged = 0;
        UselessUndoClicked = 0;
    }

    public void Push()
    {
        if (_profileManager.CurrentProfile is null) 
            throw new InvalidOperationException("No current profile found");

        _profileManager.UpdateStatistics(GameOver, HasWon, MovesMade, UndosMade);
        Reset();
    }
}
