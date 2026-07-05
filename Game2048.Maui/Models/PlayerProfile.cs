using Game2048.Core.Enums;
using Game2048.Maui.Enums;
using Game2048.Maui.Models;

namespace Game2048.Core.Models;

public class PlayerProfile
{
    public string Name { get; set; }
    public int Coins { get; set; }
    public List<GameTheme> UnlockedThemes { get;}
    public HashSet<AchievementType> Achievements { get;}
    public Dictionary<GameModeType, int> BestScores { get;}
    public Dictionary<GameModeType, GameSessionSave> Saves { get;}

    public PlayerProfile()
    {
        Name = "Player";
        Coins = 0;
        UnlockedThemes = new List<GameTheme> { GameTheme.ClassicTheme };
        Achievements = new HashSet<AchievementType>();
        BestScores = new Dictionary<GameModeType, int>();
        Saves = new Dictionary<GameModeType, GameSessionSave>();
    }
}
