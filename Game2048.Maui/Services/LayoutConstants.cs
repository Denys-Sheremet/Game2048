using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.Services;

public static class LayoutConstants
{
    public const double TileSize = 100;
    public const double GapSize = 10;
    public const double EdgePadding = 40;

    public static double GetCoordinate(int idx)
    {
        return (idx * TileSize) + ((idx + 1) * GapSize);
    }

    public static double GetBoardSize(int count)
    {
        return (count * TileSize) + ((count + 1) * GapSize);
    }

    public static double CalculateScale(double containerWidth, double containerHeight, double boardWidth, double boardHeight)
    {
        if (boardWidth <= 0 || boardHeight <= 0) return 1.0;

        return Math.Min((containerWidth - EdgePadding) / boardWidth,
                        (containerHeight - EdgePadding) / boardHeight);
    }

    public static double GetLogoWidth(double width)
    {
        return width - (EdgePadding * 2);
    }
}
