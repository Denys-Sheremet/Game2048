using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048;

internal class Tile
{
    public int Id { get; private set; }
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

    public void DefineId(int id)
    {
        if (id == 0) Id = id;
    }

    public override string ToString()
    {
        return $"{Value}";
    }
}
