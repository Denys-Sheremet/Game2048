using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.Services;

public class ActionInputQueue
{
    private readonly ConcurrentQueue<Func<Task>> _queue = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public void Enqueue(Func<Task> task)
    {
        if (_queue.Count > 2) return;

        _queue.Enqueue(task);

        Process();
    }

    private async void Process()
    {
        if (!_semaphore.Wait(0)) return;

        try
        {
            while (_queue.TryDequeue(out var task))
            {
                await task();
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Clear()
    {
        _queue.Clear();
    }
}
