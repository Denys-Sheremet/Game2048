using Game2048.Core;
using Game2048.Core.DTOs;
using Game2048.Core.Mechanics;
using Game2048.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Game2048.Maui.ViewModels;

public class GameViewModel : BindableObject
{
    private readonly Game _gameCore;
    public ObservableCollection<TileViewModel> Tiles { get; } = new();
    public Func<IEnumerable<TileTransition>, Task>? RequestAnimation;
    private bool _isAnimating;

    private int _score;
    public int Score
    {
        get => _score;
        set { _score = value; OnPropertyChanged(); }
    }

    public int Rows => _gameCore.Grid.Height;
    public int Columns => _gameCore.Grid.Width;

    public GameViewModel(int rows, int cols)
    {
        _gameCore = GameFactory.CreateStandardGame(cols, rows);
    }

    public void StartNewGame()
    {
        _gameCore.SpawnMultipleTiles(2);
        SyncTiles();
        Score = _gameCore.Grid.Score;
    }

    public void SyncTiles()
    {
        Tiles.Clear();

        for(int i = 0; i < _gameCore.Grid.Count; i++)
        {
            var tile = _gameCore.Grid[i];
            Tiles.Add(new TileViewModel(tile, tile.PosY, tile.PosX));
        }
    }

    public ICommand MoveCommand => new Command<string>(async directionStr =>
    {
        if (_isAnimating) return;

        var dir = Enum.Parse<MoveDirection>(directionStr);
        var transitions = _gameCore.Move(dir);

        if (transitions.Any())
        {
            _isAnimating = true;

            try
            {
                if(RequestAnimation is not null)
                {
                    await RequestAnimation.Invoke(transitions);
                }
                
                Score = _gameCore.Grid.Score;
            }
            finally
            {
                _isAnimating = false;
            }
            
        }
    });

    public TileViewModel? GetTileViewModelAt(int x, int y)
    {
        //take into account that access to Grid is possible using x, y as parameters
        //where x is obviously width (column) and y - height (rows)
        var tile = _gameCore.Grid[x, y];
        if (tile == null) return null;

        return new TileViewModel(tile, y, x);
    }
}
