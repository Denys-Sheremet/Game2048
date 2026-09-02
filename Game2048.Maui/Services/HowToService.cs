using Game2048.Maui.Interfaces;
using Game2048.Maui.Resources.Localization;

namespace Game2048.Maui.Services;

public class HowToService : IHowToService
{
    public string GetHowToTitle(int cardId)
    {
        string key = $"HowToCard_{cardId}_title";
        return AppResources.ResourceManager.GetString(key, AppResources.Culture) ?? $"No title for card {cardId}";
    }

    public string GetHowToDesc(int cardId)
    {
        string key = $"HowToCard_{cardId}_desc";
        return AppResources.ResourceManager.GetString(key, AppResources.Culture) ?? $"No description for card {cardId}";
    }

    public string GetHowToImageSource(int cardId)
    {
        return $"howto_card_{cardId}.png";
    }
}
