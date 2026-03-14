namespace Game2048.Core.Models;

public class Tile
{
    public int Id { get; private set; }
    public (int Id1, int Id2)? Parents { get; private set; } = null;
    public int PosX {  get; private set; } 
    public int PosY {  get; private set; }
    public int PreviousX {  get; private set; }
    public int PreviousY {  get; private set; } 
    public bool IsMerged { get; private set; }
    public int Value { get; private set; }


    public Tile(int id, int posX, int posY, int previousX, int previousY, bool isMerged, int value)
    {
        if(id < 0 || posX < 0 || posY < 0 || previousX < 0 || previousY < 0 || value < 0)
            throw new ArgumentException("Invalid tile initialization data");
        Id = id;
        PosX = posX;
        PosY = posY;
        PreviousX = previousX;
        PreviousY = previousY;
        IsMerged = isMerged;
        Value = value;
    }

    public Tile(int id, int posX, int posY, bool isMerged, int value)
    {
        if (id < 0 || posX < 0 || posY < 0 || value < 0)
            throw new ArgumentException("Invalid tile initialization data");
        Id = id;
        PosX = posX;
        PosY = posY;
        PreviousX = posX;
        PreviousY = posY;
        IsMerged = isMerged;
        Value = value;
    }

    //Simplified constructor to double the Previous parameters like Position
    //Usually when spawn a tile its Previous coordinates are equal to Position coordinates
    //IsMerged is false by default
    public Tile(int id, int posX, int posY, int value)
    {
        if (id < 0 || posX < 0 || posY < 0 || value < 0)
            throw new ArgumentException("Invalid tile initialization data");
        Id = id;
        PosX = posX;
        PosY = posY;
        PreviousX = posX;
        PreviousY = posY;
        IsMerged = false;
        Value = value;
    }

    public void SetPosition (int posX, int posY)
    {
        if (posX < 0 || posY < 0)
            throw new ArgumentException("Coordinates cannot be negative");
        PosX = posX;
        PosY = posY;
    }

    public void SetPrevious(int previousX, int previousY)
    {
        if (previousX < 0 || previousY < 0)
            throw new ArgumentException("Coordinates cannot be negative");
        PreviousX = previousX;
        PreviousY = previousY;
    }

    public void SetValue(int value)
    {
        if (value < 0)
            throw new ArgumentException("Value cannot be negative");
        Value = value;
    }

    public void SetMerged(bool isMerged)
    {
        IsMerged = isMerged;
    }

    public void SetParents(int? id1 = null, int? id2 = null)
    {
        //pattern matching
        Parents = (id1, id2) switch
        {
            (int val1, int val2) => new(val1, val2),
            _ => null
        };
    }

    //Special methods for animation handle
    public void SyncPrevious()
    {
        SetPrevious(PosX, PosY);
    }

    public void UpdatePosition(int newX, int newY)
    {
        SyncPrevious();
        SetPosition(newX, newY);
    }
}
