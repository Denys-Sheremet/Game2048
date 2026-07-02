namespace Game2048.Core.Services;

public class DisabledHistoryManager : IHistoryManager
{
    public int Count => 0;

    public void Push(StateSnapshot state) { return; }
    public StateSnapshot Pop() { throw new InvalidOperationException("This game mode cannot use Undo"); }
    public void Clear() { return; }

    public void RemoveMultiple(int count) { return; }
    public IReadOnlyList<StateSnapshot>? ToList() => null;
}
