using Game2048.Core.Models;
using Game2048.Maui.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048.Maui.Services;

public class ProfileManager : IProfileManager
{
    public PlayerProfile? CurrentProfile { get; set; }
}
