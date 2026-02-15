using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048;

internal class Tile
{
    public int Id { get; private set; }
    public (int Id1, int Id2)? Parents { get; private set; } = null;
    public int PosX {  get; private set; } 
    public int PosY {  get; private set; }
    public int TargetX {  get; private set; }
    public int TargetY {  get; private set; } 
    public bool IsMerged { get; private set; }

    public int Value { get; private set; }


    public Tile(int id, int posX, int posY, int targetX, int targetY, bool isMerged, int value)
    {
        Id = id;
        PosX = posX;
        PosY = posY;
        TargetX = targetX;
        TargetY = targetY;
        IsMerged = isMerged;
        Value = value;
    }

    public void SetPosition (int posX, int posY)
    {
        PosX = posX;
        PosY = posY;
    }

    public void SetTarget(int targetX, int targetY)
    {
        TargetX = targetX;  
        TargetY = targetY;
    }

    public void SetValue(int value)
    {
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

    public override string ToString()
    {
        return $"{Value}";
    }
}
