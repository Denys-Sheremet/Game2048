using Game2048.Maui.Achievements.Services;
using Game2048.Maui.Enums;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Views.Pages;
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

    public event Action<string, string, string>? OnAchievementSelected;

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

        OpenAchievementCommand = new Command<AchievementData>(OnOpenAchievement);
    }

    public async Task InitializeDataAsync()
    {
        if (Achievements.Count > 0) return;

        int unlockedCount = 0;

        var itemsToLoad = await Task.Run(() =>
        {
            var tempCollection = new List<AchievementData>();
            var achievementTypes = Enum.GetValues<AchievementType>();
            var unlockedAchievements = _profileManager.GetUnlockedAchievements();

            foreach (var type in achievementTypes)
            {
                string title = AchievementDataParser.GetTitle(type);
                string desc = AchievementDataParser.GetDesc(type);
                string imageName = AchievementDataParser.GetImageName(type);
                bool isUnlocked = unlockedAchievements.Contains(type);

                if (isUnlocked) unlockedCount++;

                tempCollection.Add(new AchievementData(title, desc, imageName, isUnlocked));
            }
            return tempCollection;
        });

        foreach (var item in itemsToLoad)
        {
            Achievements.Add(item);
        }

        TotalProgressFraction = (double)unlockedCount / itemsToLoad.Count;
        AchievementsCountText = $"{unlockedCount} / {itemsToLoad.Count}";
    }

    private void OnOpenAchievement(AchievementData? selected)
    {
        if (selected is null) return;

        OnAchievementSelected?.Invoke(selected.Title, selected.Desc, selected.ImageName);
    }

}
