using Game2048.Core.Enums;
using Game2048.Maui.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.Interfaces;

public interface ISettingsManager
{
    public string GetCurrentLang();
    bool LoadInitialSettings();
    void SetGameMode(GameModeType mode);
    void SetTheme(GameTheme theme);
    void SetLanguage(string langCode);
}
