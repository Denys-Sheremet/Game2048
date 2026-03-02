using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console2048;

public interface IRandomProvider
{
    int Next(int maxValue);
    int Next(int minValue, int maxValue);
}
