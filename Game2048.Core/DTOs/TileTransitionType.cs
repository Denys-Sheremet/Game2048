namespace Game2048.Core.DTOs;

public enum TileTransitionType
{
    /// <summary>Tile is just being moved on the field</summary>
    Move,
    /// <summary>Tile is to appear (regular spawn)</summary>
    Spawn,
    /// <summary>Parent tile moving to a merge point</summary>
    Merge,
    /// <summary>New tile appearing as a result of merge</summary>
    Result,
    /// <summary>One tile splitting into two (Undo of merge)</summary>
    Split,
    /// <summary>Tile to be restored after undo of Merge</summary>
    Respawn,
    /// <summary>Tile disappearing (Undo of spawn)</summary>
    Disappear,
    /// <summary>Tile stayed in place</summary>
    Stay
}
