namespace Game2048.Core.Services;

public class MultipleTileSpawner : TileSpawner, ITileSpawner
{
    private readonly int _tilesCountToSpawn;

    public MultipleTileSpawner(int tilesCountToSpawn)
    {
        if (tilesCountToSpawn <= 1) throw new ArgumentException("Invalid tiles count in spawner provided"); 
        _tilesCountToSpawn = tilesCountToSpawn;
    }

    public override bool Spawn(Grid grid, ITileRegistry registry, ref int nextId, IRandomProvider random)
    {
        bool success = false;
        for (int i = 0; i < _tilesCountToSpawn; i++)
        {
            if (!base.Spawn(grid, registry, ref nextId, random)) 
                break;

            success = true;
            nextId++;
        }
        nextId--;
        return success;
    }
}
