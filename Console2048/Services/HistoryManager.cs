namespace Game2048.Core.Services;

public class HistoryManager : IHistoryManager
{
    private readonly Stack<StateSnapshot> _stack = new();

    public int Count => _stack.Count;
    public bool IsEmpty => _stack.Count == 0;
    public void Push(StateSnapshot state) 
    {
        ArgumentNullException.ThrowIfNull(state);
        _stack.Push(state);
    } 
    public StateSnapshot? Pop() => _stack.Count > 0 ? _stack.Pop() : null;
    public void Clear() => _stack.Clear();
}
