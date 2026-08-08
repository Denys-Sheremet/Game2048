using System;
using System.Collections.Generic;
using System.Text;

namespace Game2048.Maui.Extensions;

public static class AnimationExtensions
{
    public static double GetWidthOrDefault(this VisualElement element) 
    {
        if (element.Width > 0) 
        {
            return element.Width;
        }

        var mainDisplay = DeviceDisplay.MainDisplayInfo;
        return mainDisplay.Width / mainDisplay.Density;
    }
}
