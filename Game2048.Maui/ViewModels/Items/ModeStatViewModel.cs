using Game2048.Core.Enums;

namespace Game2048.Maui.ViewModels.Items;

public partial class ModeStatViewModel : BindableObject
{
    public required GameModeType ModeType { get; init; }
    public required string ModeName { get; init; }
    public required int HighestScore { get; init; }
    public string RatingIcon { get; init; } = "profile_top_none.svg";
}
