namespace Game2048.Core.Interfaces;

public interface ITileSpawner
{
    bool Spawn(Grid grid, ITileRegistry registry, ref int nextId, IRandomProvider random);
    bool TrySpawnAt(Grid grid, ITileRegistry registry, int nextId, int x, int y, int? value, IRandomProvider? random);
    public void Reset(int resettedInitialSpawns = 2);
}
