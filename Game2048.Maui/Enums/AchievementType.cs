using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.Enums;

public enum AchievementType
{
    /// <summary>Played the first game</summary>
    FirstGame,

    /// <summary>Reached the 4 tile</summary>
    Reach4,

    /// <summary>Reached the 8 tile</summary>
    Reach8,

    /// <summary>Reached the 16 tile</summary>
    Reach16,

    /// <summary>Reached the 32 tile</summary>
    Reach32,

    /// <summary>Reached the 64 tile</summary>
    Reach64,

    /// <summary>Reached the 128 tile</summary>
    Reach128,

    /// <summary>Reached the 256 tile</summary>
    Reach256,

    /// <summary>Reached the 512 tile</summary>
    Reach512,

    /// <summary>Reached the 1024 tile</summary>
    Reach1024,

    /// <summary>Reached the 2048 tile</summary>
    Reach2048,

    /// <summary>Scored at least 1000 points in a single game</summary>
    Score1000,

    /// <summary>Scored at least 5000 points in a single game</summary>
    Score5000,

    /// <summary>Played 10 games</summary>
    Play10Games,

    /// <summary>Played 100 games</summary>
    Play100Games,

    /// <summary>Performed 100 tile merges</summary>
    Merge100Tiles,

    /// <summary>Used Undo for the first time</summary>
    FirstUndo,

    /// <summary>Won a game without using Undo</summary>
    NoUndoVictory,

    /// <summary>Claimed all achievements</summary>
    MasterOf2048
}
