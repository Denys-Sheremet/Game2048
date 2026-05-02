using Game2048.Maui.Models;

namespace Game2048.Maui.Interfaces;

public interface IGameModeService
{
    IEnumerable<GameMode> GetAvailableGameModes();
}
