namespace Game2048.Core.Interfaces;

public interface ITileRegistry : IReadOnlyTileRegistry
{
    void Register(Tile tile);
    void RegisterMany(IEnumerable<Tile> tiles);
    void Unregister(int id);
    void UnregisterMany(IEnumerable<Tile> tiles);
    void Clear();
}
