using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.Achievements.Interfaces;

public interface ISpecialAchievementChecker : IAchievementChecker
{
    bool Check(bool gameOver, bool hasWon, int movesMade, int undosMade, int tilesMerged);
}
