using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Game2048.Core.Enums;

namespace Game2048.Maui.Models;

public partial class GameMode : ObservableObject
{
    public string Title { get; set; }
    public string Desc { get; set; }
    public string GifSource { get; set; }
    public Brush CardColor { get; set; }
    public GameModeType ModeType { get; set; }

    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public GameMode(GameModeType modeType, string title, string desc, string gifSource, Brush cardColor, bool isActive) 
    {
        ModeType = modeType;
        Title = title;
        Desc = desc;
        GifSource = gifSource;
        CardColor = cardColor;
        IsActive = isActive;
    }
}
