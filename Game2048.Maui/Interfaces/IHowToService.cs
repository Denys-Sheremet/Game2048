using System;
using System.Collections.Generic;
using System.Text;

namespace Game2048.Maui.Interfaces;

public interface IHowToService
{
    string GetHowToTitle(int cardId);
    string GetHowToDesc(int cardId);
    string GetHowToImageSource(int cardId);
}
