using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using Game2048.Core.Mechanics;
using Game2048.Core.Models;
using Game2048.Core.Services;
using Game2048.Core.Interfaces;
using Xunit;

namespace Game2048.Core.Test;

public class LimitedHistoryManagerTest
{
    private int Counter = 100;
    private StateSnapshot CreateSnapshot(int score = 0)
        => new StateSnapshot(4, 4, score, new List<Tile>(), Counter++);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_ShouldThrow_ArgumentException_IfLimitIsInvalid(int limit)
    {
        Assert.Throws<ArgumentException>(() => new LimitedHistoryManager(limit));
    }

    [Fact]
    public void Push_ShouldIncreaseCount_UpToLimit()
    {
        int limit = 5;
        var manager = new LimitedHistoryManager(limit);

        for (int i = 0; i < limit; i++)
        {
            manager.Push(CreateSnapshot());
        }

        Assert.Equal(limit, manager.Count);
    }

    [Fact]
    public void Push_Throws_ArgumentNullException_IfStateIsNull()
    {
        var manager = new LimitedHistoryManager(10);
        Assert.Throws<ArgumentNullException>(() => manager.Push(null!));
    }

    [Fact]
    public void Pop_ShouldReturn_LastPushedState_AndDecreaseCount()
    {
        var manager = new LimitedHistoryManager(10);
        var s1 = CreateSnapshot(100);
        var s2 = CreateSnapshot(200);

        manager.Push(s1);
        manager.Push(s2);

        Assert.Equal(200, manager.Pop()!.Score);
        Assert.Equal(1, manager.Count);
        Assert.Equal(100, manager.Pop()!.Score);
        Assert.Equal(0, manager.Count);
    }

    [Fact]
    public void Pop_ShouldReturnNull_IfHistoryIsEmpty()
    {
        var manager = new LimitedHistoryManager(10);
        Assert.Null(manager.Pop());
    }

    [Fact]
    public void Push_Should_EvictOldestState_WhenLimitIsReached()
    {
        int limit = 2;
        var manager = new LimitedHistoryManager(limit);
        var s1 = CreateSnapshot(10);
        var s2 = CreateSnapshot(20);
        var s3 = CreateSnapshot(30);

        manager.Push(s1);
        manager.Push(s2);
        manager.Push(s3);

        Assert.Equal(limit, manager.Count);

        Assert.Equal(30, manager.Pop()!.Score);
        Assert.Equal(20, manager.Pop()!.Score);

        Assert.Null(manager.Pop());
    }

    [Fact]
    public void Clear_Should_EmptyTheHistory()
    {
        var manager = new LimitedHistoryManager(10);
        manager.Push(CreateSnapshot());
        manager.Push(CreateSnapshot());

        manager.Clear();

        Assert.Equal(0, manager.Count);
        Assert.Null(manager.Pop());
    }

}
