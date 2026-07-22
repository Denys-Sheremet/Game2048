using Game2048.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.Interfaces;

public interface ISettingsManager
{
    bool LoadInitialSettings();
    void SetGameMode(GameModeType mode);
    void SetTheme(string themeName);
    void SetLanguage(string langCode);
}
