using Game2048.Maui.Enums;
using Game2048.Maui.Extensions;
using Game2048.Maui.Interfaces;

namespace Game2048.Maui.Services;

public class StoreManager : IStoreManager
{
    private readonly IProfileManager _profileManager;

    public StoreManager(IProfileManager profileManager, IThemesManager themesManager)
    {
        _profileManager = profileManager;
    }

    public bool TryBuyTheme(GameTheme theme)
    {
        if (_profileManager.IsThemeUnlocked(theme)) return false;

        int themePrice = theme.GetPrice();
        int currentBalance = _profileManager.CurrentCoins;

        if (currentBalance < themePrice) return false;

        _profileManager.SpendCoins(themePrice);
        _profileManager.UnlockTheme(theme);

        return true;
    }
}
