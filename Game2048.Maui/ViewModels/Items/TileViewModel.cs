using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game2048.Core.Models;

namespace Game2048.Maui.ViewModels.Items;

public partial class TileViewModel : BindableObject
{
    private readonly Tile _model;
    
    public int Row {  get; set; }
    public int Column { get; set; }

    public int Value => _model.Value;
    public int Id => _model.Id;

    public int FontSize => Value switch
    {
        < 100 => 48,
        < 1000 => 38,
        < 10000 => 28,
        _ => 32
    };


    public TileViewModel(Tile model, int row, int col)
    {
        _model = model;
        Row = row;
        Column = col;
    }

    public void SyncData()
    {
        Row = _model.PosY; 
        Column = _model.PosX;
    }
}
