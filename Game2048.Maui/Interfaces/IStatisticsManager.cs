using Game2048.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.Interfaces;

public interface IStatisticsManager
{
    void Moved();
    void Undone();
    void GameEnded(bool hasWon = false);
    void Reset();
    void Push();
}
