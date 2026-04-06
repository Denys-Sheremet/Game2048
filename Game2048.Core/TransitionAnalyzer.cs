using Game2048.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Core;

public static class TransitionAnalyzer
{
    public static List<TileTransition> Analyze(StateSnapshot before, StateSnapshot after)
    {
        Dictionary<int, TileSnapshot> beforeDict = before.TileSnapshots.ToDictionary(ts => ts.Id);
        Dictionary<int, TileSnapshot> afterDict = after.TileSnapshots.ToDictionary(ts => ts.Id);

        Dictionary<int, TileSnapshot> parentToChild = new();

        List<TileTransition> transitions = new List<TileTransition>();

        //first cycle for Move, Stay, Spawn, Result identification
        foreach (TileSnapshot ts in afterDict.Values) 
        {
            int id = ts.Id;
            TileTransitionType type;
            (int x, int y) from = (ts.PosX, ts.PosY);
            (int x, int y) to = from;
            (int? id1, int? id2) parents = (null, null);

            beforeDict.TryGetValue(id, out TileSnapshot? beforeTile);

            if (beforeTile is not null)
            {
                if (
                    beforeTile.PosX == ts.PosX
                    &&
                    beforeTile.PosY == ts.PosY
                   )
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

        //second cycle for Merge, Split, Disappear identification
        foreach (TileSnapshot ts in beforeDict.Values) 
        {
            if (afterDict.ContainsKey(ts.Id)) continue;

            int id = ts.Id;
            TileTransitionType type;
            (int x, int y) from = (ts.PosX, ts.PosY);
            (int x, int y) to = from;
            (int? id1, int? id2) parents = (null, null);

            if(parentToChild.TryGetValue(id, out TileSnapshot? child))
            {
                type = TileTransitionType.Merge;
                to.x = child.PosX;
                to.y = child.PosY;
            }
            else if (ts.Parents is not null)
            {
                type = TileTransitionType.Split;
                parents.id1 = ts.Parents.Value.Id1;
                parents.id2 = ts.Parents.Value.Id2;
            }
            else
            {
                type = TileTransitionType.Disappear;
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

        return transitions;
    }
}
