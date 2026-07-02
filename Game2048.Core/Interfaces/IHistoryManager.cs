namespace Game2048.Core.Interfaces;

public interface IHistoryManager
{
    int Count { get; }
    bool IsEmpty => Count == 0;
    void Push(StateSnapshot state);
    StateSnapshot? Pop();
    void Clear();
    void RemoveMultiple(int count);
    IReadOnlyList<StateSnapshot>? ToList();
}
