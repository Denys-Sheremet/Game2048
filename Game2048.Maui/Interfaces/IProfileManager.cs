using Game2048.Core.Enums;
using Game2048.Maui.Enums;
using Game2048.Core.Models;
using Game2048.Maui.Models;


namespace Game2048.Maui.Interfaces;

public interface IProfileManager
{
    PlayerProfile? CurrentProfile { get; }
    void NewProfile();
    Task<bool> TryLoadProfileFromSave();
    Task SaveCurrentProfileAsync();
    void UpdateStatistics(bool gameEnded, bool hasWon, int movesMade, int undosMade);
    void SetCurrentProfile(PlayerProfile profile);
    void SaveCurrentGame(GameModeType gameMode, StateSnapshot currentState, IReadOnlyList<StateSnapshot>? history);
    GameSessionSave? LoadCurrentGame(GameModeType gameMode);
    void SaveBestScore(GameModeType gameMode, int bestScore);
    int? GetBestScore(GameModeType gameMode);
    void UnlockAchievement(AchievementType achievement);
    HashSet<AchievementType> GetUnlockedAchievements();
    void UnlockTheme(GameTheme theme);
    List<GameTheme> GetUnlockedThemes();
    void EarnCoins(int amount);
    bool SpendCoins(int amount);
    void SetPlayerName(string name);
    string GetPlayerName();
}
