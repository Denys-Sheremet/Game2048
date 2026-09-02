using Game2048.Maui.Interfaces;
using Game2048.Maui.Services;
using Game2048.Maui.ViewModels.Items;
using System.Collections.ObjectModel;

namespace Game2048.Maui.ViewModels;

public partial class HowToViewModel : BindableObject
{
    private readonly IHowToService _howToService;
    public ObservableCollection<HowToCardViewModel> HowToCards { get; } = new();

    private const int CardsCount = 6;
    
    public HowToViewModel(IHowToService howToService)
    {
        _howToService = howToService;

        InitializeHowToCards();
    }

    private void InitializeHowToCards()
    {
        for (int i = 1; i <= CardsCount; i++)
        {
            HowToCards.Add(new HowToCardViewModel
            {
                Id = i,
                Title = _howToService.GetHowToTitle(i),
                Description = _howToService.GetHowToDesc(i),
                ImageSource = _howToService.GetHowToImageSource(i)
            });
        }
    }
}
