using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;


namespace Game2048.Maui.Services;

public class ActionInputQueue
{
    private readonly ConcurrentQueue<Func<Task>> _queue = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ILogger<ActionInputQueue> _logger;

    public ActionInputQueue(ILogger<ActionInputQueue>? logger = null)
    {
        _logger = logger ?? NullLogger<ActionInputQueue>.Instance;
    }

    public void Enqueue(Func<Task> task)
    {
        if (_queue.Count > 3) return;

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
                try
                {
                    await task();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception occurred while executing queued action");
                }
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
