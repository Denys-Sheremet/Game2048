using Game2048.Core.Models;
using Game2048.Maui.Models;
using Game2048.Core.Enums;
using Game2048.Maui.Enums;
using Game2048.Maui.Interfaces;


namespace Game2048.Maui.Services;

public class ProfileManager : IProfileManager
{
    public PlayerProfile? CurrentProfile { get; private set; }
    private readonly ISaveService _saveService;

    public ProfileManager(ISaveService saveService)
    {
        _saveService = saveService;
    }
    public void NewProfile()
    {
        CurrentProfile = new();
    }

    //global save / load
    public async Task<bool> TryLoadProfileFromSave()
    {
        var profile = await _saveService.LoadProfileAsync();

        if (profile is not null)
        {
            CurrentProfile = profile;
            return true;
        }
        return false;
    }

    
    public async Task SaveCurrentProfileAsync()
    {
        if (CurrentProfile is not null)
        {
            await _saveService.SaveProfileAsync(CurrentProfile);
        }
    }

    private PlayerProfile GetValidProfile()
    {
        return CurrentProfile ?? throw new InvalidOperationException("No current profile found");
    }

    public void SetCurrentProfile(PlayerProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        CurrentProfile = profile;
    }

    //Statistics
    public void UpdateStatistics(bool gameEnded, bool hasWon, int movesMade, int undosMade)
    {
        var profile = GetValidProfile();
        var stats = profile.GlobalPlayerStatistics;
        stats.TotalGamesPlayed = gameEnded ? stats.TotalGamesPlayed + 1 : stats.TotalGamesPlayed;
        stats.TotalGamesWon = hasWon ? stats.TotalGamesWon + 1 : stats.TotalGamesWon;
        stats.TotalMovesMade += movesMade;
        stats.TotalUndosUsed += undosMade;
    } 

    //Local save & load
    public void SaveCurrentGame(GameModeType gameMode, StateSnapshot currentState, IReadOnlyList<StateSnapshot>? history)
    {
        var profile = GetValidProfile();
        profile.Saves[gameMode] = new GameSessionSave
        {
            LastState = currentState,
            History = history?.ToList()
        };
    }

    public GameSessionSave? LoadCurrentGame(GameModeType gameMode)
    {
        var profile = GetValidProfile();
        if (profile.Saves.TryGetValue(gameMode, out GameSessionSave? save))
        {
            return save;
        }
        return null;
    }

    //Best score
    public void SaveBestScore(GameModeType gameMode, int bestScore)
    {
        var profile = GetValidProfile();
        profile.BestScores[gameMode] = bestScore;
    }

    public int? GetBestScore(GameModeType gameMode)
    {
        var profile = GetValidProfile();
        if (profile.BestScores.TryGetValue(gameMode, out int bestScore))
        {
            return bestScore;
        }
        return null;
    }

    //Achievements
    public void UnlockAchievement(AchievementType achievement)
    {
        var profile = GetValidProfile();

        profile.Achievements.Add(achievement);
    }

    public HashSet<AchievementType> GetUnlockedAchievements()
    {
        var profile = GetValidProfile();
        return profile.Achievements;
    }

    //Themes
    public void UnlockTheme(GameTheme theme)
    {
        var profile = GetValidProfile();
        if (!profile.UnlockedThemes.Contains(theme))
        {
            profile.UnlockedThemes.Add(theme);
        }
    }

    public List<GameTheme> GetUnlockedThemes()
    {
        var profile = GetValidProfile();
        return profile.UnlockedThemes;
    }

    //Coins
    public void EarnCoins(int amount)
    {
        var profile = GetValidProfile();
        if (amount <= 0) throw new InvalidOperationException("Amount of coins to add should be more than 0");
        profile.Coins += amount;
    }

    public bool SpendCoins(int amount)
    {
        var profile = GetValidProfile();
        if (profile.Coins >= amount)
        {
            profile.Coins -= amount;
            return true;
        }
        return false;
    }

    //Name
    public void SetPlayerName(string name)
    {
        var profile = GetValidProfile();
        profile.Name = name;
    }

    public string GetPlayerName()
    {
        var profile = GetValidProfile();
        return profile.Name;
    }
}
