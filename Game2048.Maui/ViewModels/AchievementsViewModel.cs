using Game2048.Maui.Achievements.Services;
using Game2048.Maui.Enums;
using Game2048.Maui.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Game2048.Maui.ViewModels;

public record AchievementData
{
    public string Title { get; set; }
    public string Desc { get; set; }
    public string ImageName { get; set; }
    public bool IsUnlocked { get; set; }

    public AchievementData(string title, string desc, string imageName, bool isUnlocked)
    {
        Title = title; Desc = desc; ImageName = imageName; IsUnlocked = isUnlocked;
    }
}

public partial class AchievementsViewModel : BindableObject
{
    public ObservableCollection<AchievementData> Achievements { get; } = new();
    private readonly IProfileManager _profileManager;

    private double _totalProgressFraction;
    private string _achievementsCountText = "0 / 0";

    public double TotalProgressFraction
    {
        get => _totalProgressFraction;
        set
        {
            if (_totalProgressFraction != value)
            {
                _totalProgressFraction = value;
                OnPropertyChanged();
            }
        }
    }

    public string AchievementsCountText
    {
        get => _achievementsCountText;
        set
        {
            if (_achievementsCountText != value)
            {
                _achievementsCountText = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand OpenAchievementCommand { get; }

    public AchievementsViewModel(IProfileManager profileManager)
    {
        _profileManager = profileManager;
        OpenAchievementCommand = new Command<AchievementData>(async (selected) => await OnOpenAchievementAsync(selected));

        LoadAchievements();
    }

    private void LoadAchievements()
    {
        Achievements.Clear();
        var achievementTypes = Enum.GetValues<AchievementType>();
        var unlockedAchievements = _profileManager.GetUnlockedAchievements();
        int unlockedCount = 0;

        foreach (var type in achievementTypes)
        {
            string title = AchievementDataParser.GetTitle(type);
            string desc = AchievementDataParser.GetDesc(type);
            string imageName = AchievementDataParser.GetImageName(type);

            bool isUnlocked = unlockedAchievements.Contains(type);

            if (isUnlocked) unlockedCount++;

            Achievements.Add(new AchievementData(title, desc, imageName, isUnlocked));
        }

        if (Achievements.Count > 0)
        {
            TotalProgressFraction = (double)unlockedCount / Achievements.Count;
            AchievementsCountText = $"{unlockedCount} / {Achievements.Count}";
        }
    }

    private async Task OnOpenAchievementAsync(AchievementData? selected)
    {
        if (selected is null) return;

        
    }

}
