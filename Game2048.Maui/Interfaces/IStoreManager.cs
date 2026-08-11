using Game2048.Maui.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game2048.Maui.Interfaces;

public interface IStoreManager
{
    bool TryBuyTheme(GameTheme theme);
}
