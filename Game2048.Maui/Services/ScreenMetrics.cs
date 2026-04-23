using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.Services;

public static class ScreenMetrics
{
    public static double Width { get; private set; }

    public static double Height { get; private set; }

    public static double Density { get; private set; }

    static ScreenMetrics()
    {
        UpdateMetrics();
    }

    public static double GetPercentFromWidth(double percent) => 
        Width * (percent / 100.0);

    public static double GetPercentFromHeight(double percent) =>
        Height * (percent / 100.0);


    public static void UpdateMetrics()
    {
        var info = DeviceDisplay.Current.MainDisplayInfo;

        Density = info.Density;

        Width = info.Width / (Density > 0 ? Density : 1);
        Height = info.Height / (Density > 0 ? Density : 1);
    }
}
