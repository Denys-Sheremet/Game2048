using Game2048.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.Interfaces;

public interface IProfileManager
{
    PlayerProfile? CurrentProfile { get; set; }
    void NewProfile();
}
