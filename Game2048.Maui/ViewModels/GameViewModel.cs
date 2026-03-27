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

        Score = _gameCore.Grid.Score;

        SyncTiles();
    }

    public void SyncTiles()
    {
        Tiles.Clear();

        for(int i = 0; i < _gameCore.Grid.Count; i++)
        {
            var tile = _gameCore.Grid[i];
            var tileViewModel = new TileViewModel(tile, tile.PosY, tile.PosX);
            Tiles.Add(tileViewModel);
        }
    }

    public ICommand MoveCommand => new Command<string>(directionStr =>
    {
        var dir = Enum.Parse<MoveDirection>(directionStr);

        var transitions = _gameCore.Move(dir);

        if (transitions.Any())
        {
            //ApplyMoveTransitions(transitions);
            SyncTiles();
            Score = _gameCore.Grid.Score;
        }
    });

    private async void ApplyMoveTransitions(IEnumerable<TileTransition> transitions)
    {
        var movingTasks = new List<Task>();

        foreach (var tr in transitions.Where(x => x.Type == TileTransitionType.Move ||
                                                  x.Type == TileTransitionType.Merge))
        {
            var tileVM = Tiles.FirstOrDefault(t => t.Id == tr.TileId);

            if (tileVM != null)
            {
                tileVM.UpdateBounds(tr.ToY, tr.ToX);
            }
        }

        await Task.Delay(150);

        foreach (var tr in transitions)
        {
            if (tr.Type == TileTransitionType.Merge || tr.Type == TileTransitionType.Disappear)
            {
                var toRemove = Tiles.FirstOrDefault(t => t.Id == tr.TileId);
                if (toRemove != null) Tiles.Remove(toRemove);
            }

            if (tr.Type == TileTransitionType.Result || tr.Type == TileTransitionType.Spawn)
            {
                var coreTile = _gameCore.Grid[tr.ToX, tr.ToY];
                if (coreTile != null && !Tiles.Any(t => t.Id == coreTile.Id))
                {
                    Tiles.Add(new TileViewModel(coreTile, tr.ToY, tr.ToX));
                }
            }
        }
    }
}
