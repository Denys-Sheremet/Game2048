namespace Game2048.Core.Interfaces;

public interface IReadOnlyTileRegistry
{
    int Count {  get; }
    Tile? this[int id] { get; }
}
