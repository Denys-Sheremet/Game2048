using Game2048.Core.DTOs;
using Game2048.Core.Models;
using Xunit;

namespace Game2048.Core.Test;

public class TransitionAnalyzerTest
{
    [Fact]
    public void Analyze_ShouldIdentifyMergeAndSpawn()
    {
        TileSnapshot tile1 = new TileSnapshot(1, 0, 0, 2, null);
        TileSnapshot tile2 = new TileSnapshot(2, 0, 1, 2, null);
        StateSnapshot before = new StateSnapshot(4, 4, 0, new List<TileSnapshot> { tile1, tile2 }, 3);

        TileSnapshot tile3 = new TileSnapshot(3, 0, 0, 4, (1, 2));
        TileSnapshot tile4 = new TileSnapshot(4, 3, 3, 2, null);
        StateSnapshot after = new StateSnapshot(4, 4, 4, new List<TileSnapshot> { tile3, tile4 }, 5);

        List<TileTransition> result = TransitionAnalyzer.Analyze(before, after);

        Assert.Equal(4, result.Count);

        Assert.Contains(result, t => t.TileId == 1 && t.Type == TileTransitionType.Merge && t.ToX == 0 && t.ToY == 0);
        Assert.Contains(result, t => t.TileId == 2 && t.Type == TileTransitionType.Merge && t.ToX == 0 && t.ToY == 0);
        Assert.Contains(result, t => t.TileId == 3 && t.Type == TileTransitionType.Result && t.FromX == 0 && t.FromY == 0);
        Assert.Contains(result, t => t.TileId == 4 && t.Type == TileTransitionType.Spawn && t.ToX == 3 && t.ToY == 3);
    }

    [Fact]
    public void Analyze_ShouldIdentifyUndoSplit()
    {
        TileSnapshot tileMerged = new TileSnapshot(3, 0, 0, 4, (1, 2));
        StateSnapshot before = new StateSnapshot(4, 4, 4, new List<TileSnapshot> { tileMerged }, 5);

        TileSnapshot tile1 = new TileSnapshot(1, 0, 0, 2, null);
        TileSnapshot tile2 = new TileSnapshot(2, 0, 1, 2, null);
        StateSnapshot after = new StateSnapshot(4, 4, 0, new List<TileSnapshot> { tile1, tile2 }, 3);

        List<TileTransition> result = TransitionAnalyzer.Analyze(before, after);

        Assert.Contains(result, t => t.TileId == 3 && t.Type == TileTransitionType.Split);
        Assert.Contains(result, t => t.TileId == 1 && t.Type == TileTransitionType.Spawn);
        Assert.Contains(result, t => t.TileId == 2 && t.Type == TileTransitionType.Spawn);
    }

    [Fact]
    public void Analyze_StayAndMove_ShouldIdentifyCorrectTypes()
    {
        TileSnapshot tile1 = new TileSnapshot(1, 0, 0, 2, null);
        TileSnapshot tile2 = new TileSnapshot(2, 0, 1, 2, null);
        StateSnapshot before = new StateSnapshot(4, 4, 0, new List<TileSnapshot> { tile1, tile2 }, 3);

        TileSnapshot tile1Moved = new TileSnapshot(1, 0, 2, 2, null);
        TileSnapshot tile2Stayed = new TileSnapshot(2, 0, 1, 2, null);
        StateSnapshot after = new StateSnapshot(4, 4, 0, new List<TileSnapshot> { tile1Moved, tile2Stayed }, 3);

        List<TileTransition> result = TransitionAnalyzer.Analyze(before, after);

        Assert.Contains(result, t => t.TileId == 1 && t.Type == TileTransitionType.Move && t.FromY == 0 && t.ToY == 2);
        Assert.Contains(result, t => t.TileId == 2 && t.Type == TileTransitionType.Stay);
    }

    [Fact]
    public void Analyze_Disappear_ShouldIdentifySimpleRemoval()
    {
        TileSnapshot tile1 = new TileSnapshot(1, 0, 0, 2, null);
        StateSnapshot before = new StateSnapshot(4, 4, 0, new List<TileSnapshot> { tile1 }, 2);

        StateSnapshot after = new StateSnapshot(4, 4, 0, new List<TileSnapshot>(), 2);

        List<TileTransition> result = TransitionAnalyzer.Analyze(before, after);

        Assert.Single(result);
        Assert.Contains(result, t => t.TileId == 1 && t.Type == TileTransitionType.Disappear);
    }

    [Fact]
    public void Analyze_EmptyStates_ShouldReturnEmptyList()
    {
        StateSnapshot before = new StateSnapshot(4, 4, 0, new List<TileSnapshot>(), 1);
        StateSnapshot after = new StateSnapshot(4, 4, 0, new List<TileSnapshot>(), 1);

        List<TileTransition> result = TransitionAnalyzer.Analyze(before, after);

        Assert.Empty(result);
    }

    [Fact]
    public void Analyze_MultipleMergesInOneTurn()
    {
        StateSnapshot before = new StateSnapshot(4, 4, 0, new List<TileSnapshot>
        {
            new TileSnapshot(1, 0, 0, 2), new TileSnapshot(2, 0, 1, 2),
            new TileSnapshot(3, 1, 0, 2), new TileSnapshot(4, 1, 1, 2)
        }, 5);

        StateSnapshot after = new StateSnapshot(4, 4, 8, new List<TileSnapshot>
        {
            new TileSnapshot(5, 0, 0, 4, (1, 2)),
            new TileSnapshot(6, 1, 0, 4, (3, 4))
        }, 7);

        List<TileTransition> result = TransitionAnalyzer.Analyze(before, after);

        Assert.Equal(6, result.Count);
        Assert.Equal(2, result.Count(t => t.Type == TileTransitionType.Result));
        Assert.Equal(4, result.Count(t => t.Type == TileTransitionType.Merge));
    }
}