using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game2048.Core.Models;

namespace Game2048.Maui.ViewModels;

public class TileViewModel : BindableObject
{
    private readonly Tile _model;
    private Rect _bounds;

    public int Value => _model.Value;
    public int Id => _model.Id;

    public Rect Bounds
    {
        get => _bounds;
        set { _bounds = value; OnPropertyChanged(); }
    }

    public TileViewModel(Tile model, int row, int col)
    {
        _model = model;
        UpdateBounds(row, col);
    }

    public void UpdateBounds(int row, int col)
    {
        const double tileSize = 100;
        const double gapSize = 10;

        double x = (col * tileSize) + ((col + 1) * gapSize);
        double y = (row * tileSize) + ((row + 1) * gapSize);

        Bounds = new Rect(x, y, tileSize, tileSize);
    }
}
