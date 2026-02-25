using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048;

internal interface IHistoryManager
{
    int Count { get; }
    bool IsEmpty => Count == 0;
    void Push(StateSnapshot state);
    StateSnapshot? Pop();
    void Clear();
}
