using System;
using System.Collections.Generic;
using System.Linq;
using Game2048.Core.Mechanics;
using Game2048.Core.Models;
using Game2048.Core.Services;
using Game2048.Core.Interfaces;

namespace Game2048.Core.Test;

public class HistoryManagerTest
{
    [Fact]
    public void HistoryManager_Init_With_Zero_Count_And_IsEmpty_True()
    {
        HistoryManager manager = new HistoryManager();
        Assert.Equal(0, manager.Count);
        Assert.True(manager.IsEmpty);
    }

    [Fact]
    public void Push_Throws_ArgumentNullException_IfStateIsNull()
    {
        HistoryManager manager = new HistoryManager();
        Assert.Throws<ArgumentNullException>(() => manager.Push(null!));
    }

    [Fact]
    public void HistoryManager_PushAndPop_Snapshots_In_LIFO_Order()
    {
        HistoryManager manager = new HistoryManager();
        StateSnapshot s1 = new StateSnapshot(1, 1, 0, new[]{new Tile(1, 0, 0, 2)}, 100);
        StateSnapshot s2 = new StateSnapshot(2, 2, 0, new[]{new Tile(2, 1, 1, 2)}, 101);
        StateSnapshot s3 = new StateSnapshot(3, 3, 0, new[]{new Tile(3, 2, 2, 2)}, 102);

        manager.Push(s1);
        manager.Push(s2);
        manager.Push(s3);

        Assert.Equal(s3, manager.Pop());
        Assert.Equal(s2, manager.Pop());
        Assert.Equal(s1, manager.Pop());
    }

    [Fact]
    public void HistoryManager_ReturnsNull_Instead_OfException_When_History_IsEmpty()
    {
        HistoryManager manager = new HistoryManager();
        StateSnapshot? ss = manager.Pop();

        Assert.Null(ss);
    }

    [Fact]
    public void Clear_Method_SetsManagers_Properties_ToDefault()
    {
        HistoryManager manager = new HistoryManager();
        StateSnapshot s1 = new StateSnapshot(1, 1, 0, new[] { new Tile(1, 0, 0, 2) }, 100);
        StateSnapshot s2 = new StateSnapshot(2, 2, 0, new[] { new Tile(2, 1, 1, 2) }, 101);
        StateSnapshot s3 = new StateSnapshot(3, 3, 0, new[] { new Tile(3, 2, 2, 2) }, 102);

        manager.Push(s1);
        manager.Push(s2);
        manager.Push(s3);

        Assert.False(manager.IsEmpty);
        Assert.Equal(3, manager.Count);
        Assert.NotNull(manager.Pop());

        manager.Clear();

        Assert.True(manager.IsEmpty);
        Assert.Equal(0, manager.Count);
        Assert.Null(manager.Pop());
    }
}
