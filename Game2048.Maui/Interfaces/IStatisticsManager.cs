using Game2048.Maui.Services;

namespace Game2048.Maui.Interfaces;

public interface IStatisticsManager
{
    StatisticsManager GetStatisticsManager();
    void Moved();
    void Merged(int count);
    void Respawned(int count);
    void Undone();
    void GameEnded(bool hasWon = false);
    void Reset();
    void Push();
}
