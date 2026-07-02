namespace Game2048.Core.Services;

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
        ArgumentNullException.ThrowIfNull(state);

        _list.AddLast(state);
        if (Count > _limit)
        {
            _list.RemoveFirst();
        } 
    }

    public StateSnapshot? Pop()
    {
        if(Count > 0)
        {
            StateSnapshot ss = _list.Last!.Value;
            _list.RemoveLast();
            return ss;
        }
        return null;
    }

    public void Clear() => _list.Clear();

    public void RemoveMultiple(int count)
    {
        int actualPopCount = Math.Min(count, _list.Count);

        for (int i = 0; i < actualPopCount; i++)
        {
            _list.RemoveLast();
        }
    }

    public IReadOnlyList<StateSnapshot>? ToList() => _list.ToList();
}
