using Game2048.Core.Enums;
using Game2048.Core.Models;
using Game2048.Maui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.Interfaces;

public interface ISaveService
{
    Task SaveProfileAsync(PlayerProfile profile);
    Task<PlayerProfile?> LoadProfileAsync();
    void SaveCurrentGame(GameModeType gameMode, StateSnapshot currentState, IReadOnlyList<StateSnapshot>? history);
    GameSessionSave? LoadCurrentGame(GameModeType gameMode);
}
