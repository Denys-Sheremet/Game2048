using Game2048.Core.Enums;
using Game2048.Maui.Enums;
using Game2048.Core.Models;
using Game2048.Maui.Models;


namespace Game2048.Maui.Interfaces;

public interface IProfileManager
{
    PlayerProfile? CurrentProfile { get; }
    event Action<GameTheme>? ThemeUnlocked;
    event Action? StatisticsUpdated;
    int CurrentCoins { get; }
    void NewProfile();
    Task<bool> TryLoadProfileFromSave();
    Task SaveCurrentProfileAsync();
    void UpdateStatistics(bool gameEnded, bool hasWon, int movesMade, int undosMade);
    PlayerStatistics GetGlobalStatistics();
    void SetCurrentProfile(PlayerProfile profile);
    void SaveCurrentGame(GameModeType gameMode, StateSnapshot currentState, IReadOnlyList<StateSnapshot>? history);
    GameSessionSave? LoadCurrentGame(GameModeType gameMode);
    void SaveBestScore(GameModeType gameMode, int bestScore);
    int? GetBestScore(GameModeType gameMode);
    bool UnlockAchievement(AchievementType achievement);
    HashSet<AchievementType> GetUnlockedAchievements();
    void UnlockTheme(GameTheme theme);
    HashSet<GameTheme> GetUnlockedThemes();
    bool IsThemeUnlocked(GameTheme theme);
    bool IsAchievementUnlocked(AchievementType achievement);
    void EarnCoins(int amount);
    bool SpendCoins(int amount);
    void SetPlayerName(string name);
    string GetPlayerName();

    event Action<int>? CoinsChanged;
}
