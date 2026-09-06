using Game2048.Maui.Services;
using Game2048.Core.Enums;

namespace Game2048.Maui.Interfaces;

public interface IStatisticsManager
{
    bool GameOver { get; }
    bool HasWon { get; }
    int MovesMade { get; }
    int UndosMade { get; }
    int TilesMerged { get; }
    int UselessUndoClicked { get; }
    GameModeType CurrentGameMode { get; }
    StatisticsManager GetStatisticsManager();
    void SetCurrentGameMode(GameModeType gameMode);
    void Moved();
    void Merged(int count);
    void Respawned(int count);
    void Undone();
    void UselessUndone();
    void GameEnded(bool hasWon = false);
    void Reset();
    void Push();
}
