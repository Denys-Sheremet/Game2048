namespace Game2048.Core.Services;

public class DefaultRandomProvider : IRandomProvider
{
    private readonly Random _random = new();
    
    public int Next(int maxValue) => _random.Next(maxValue);
    public int Next(int minValue, int maxValue) => _random.Next(minValue, maxValue);
}
