namespace Game2048.Core.Logic;

public static class TransitionAnalyzer
{
    public static List<TileTransition> Analyze(StateSnapshot before, StateSnapshot after, bool isUndo = false)
    {
        if (isUndo)
        {
            List<TileTransition> invertedTransitions = Analyze(after, before, isUndo: false);
            return InvertTransitions(invertedTransitions);
        }

        Dictionary<int, TileSnapshot> beforeDict = before.TileSnapshots.ToDictionary(ts => ts.Id);
        Dictionary<int, TileSnapshot> afterDict = after.TileSnapshots.ToDictionary(ts => ts.Id);

        Dictionary<int, TileSnapshot> parentToChild = new();

        List<TileTransition> transitions = new();

        //for identifying Stay, Move, Spawn and Result
        foreach (TileSnapshot ts in afterDict.Values)
        {
            int id = ts.Id;
            TileTransitionType type;
            (int x, int y) from = (ts.PosX, ts.PosY);
            (int x, int y) to = from;
            (int? id1, int? id2) parents = (null, null);

            if (beforeDict.TryGetValue(id, out TileSnapshot? beforeTile))
            {
                if (beforeTile.PosX == ts.PosX && beforeTile.PosY == ts.PosY)
                {
                    type = TileTransitionType.Stay;
                }
                else
                {
                    type = TileTransitionType.Move;
                    from.x = beforeTile.PosX;
                    from.y = beforeTile.PosY;
                }
            }
            else
            {
                if (ts.Parents is null)
                {
                    type = TileTransitionType.Spawn;
                }
                else
                {
                    type = TileTransitionType.Result;
                    parents.id1 = ts.Parents.Value.Id1;
                    parents.id2 = ts.Parents.Value.Id2;
                    parentToChild[ts.Parents.Value.Id1] = ts;
                    parentToChild[ts.Parents.Value.Id2] = ts;
                }
            }

            transitions.Add(new TileTransition
                (
                    id, 
                    type, 
                    from.x, 
                    from.y, 
                    to.x, 
                    to.y, 
                    parents.id1, 
                    parents.id2
                ));
        }

        //for identifying Merge
        foreach (TileSnapshot ts in beforeDict.Values)
        {
            if (afterDict.ContainsKey(ts.Id)) continue;

            if (parentToChild.TryGetValue(ts.Id, out TileSnapshot? child))
            {
                transitions.Add(new TileTransition
                (
                    ts.Id,
                    TileTransitionType.Merge,
                    ts.PosX,
                    ts.PosY,
                    child.PosX,
                    child.PosY,
                    null,
                    null
                ));
            }
        }
        return transitions;
    }

    private static List<TileTransition> InvertTransitions(List<TileTransition> transitions)
    {
        List<TileTransition> inverted = new(transitions.Count);
        for (int i = 0; i < transitions.Count; i++)
        {
            TileTransition transition = transitions[i];
            TileTransitionType type = transition.Type switch
            {
                TileTransitionType.Spawn => TileTransitionType.Disappear,
                TileTransitionType.Result => TileTransitionType.Split,
                TileTransitionType.Merge => TileTransitionType.Respawn,
                TileTransitionType.Move => TileTransitionType.Move,
                TileTransitionType.Stay => TileTransitionType.Stay,
                _ => default
            };

            inverted.Add(new TileTransition
                (
                    transition.TileId,
                    type,
                    transition.ToX,
                    transition.ToY,
                    transition.FromX,
                    transition.FromY,
                    transition.ParentId1,
                    transition.ParentId2
                ));
        }
        return inverted;
    }
}
