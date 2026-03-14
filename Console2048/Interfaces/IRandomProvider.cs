namespace Game2048.Core.Interfaces;

public interface IRandomProvider
{
    int Next(int maxValue);
    int Next(int minValue, int maxValue);
}
