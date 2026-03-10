using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048;

public class LimitedHistoryManager : IHistoryManager
{
    private readonly LinkedList<StateSnapshot> _list = new();
    private readonly int _limit;

    public int Count => _list.Count;

    public LimitedHistoryManager(int limit)
    {
        if (limit <= 0) throw new ArgumentException("limit should be more than 0");
        _limit = limit;
    }

    public void Push(StateSnapshot state)
    {

    }

    public void Pop()
    {

    }

    public void Clear() => _list.Clear();
}
