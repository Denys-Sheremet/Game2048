using Game2048.Core.DTOs;
using Game2048.Maui.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace Game2048.Maui.Views;

public partial class GameView : ContentPage
{
    private double _targetWidth;
    private double _targetHeight;
    private const double tileSize = 100;
    private const double gapSize = 10;

    private GameViewModel _viewModel;

    private readonly Dictionary<int, TileView> _tileViews = new();

    public GameView()
    {
        InitializeComponent();

        _viewModel = new GameViewModel(4, 4);
        BindingContext = _viewModel;

        _viewModel.TilesMoved += async (transitions) =>
        {
            await ApplyMoveTransitionsAsync(transitions);
        };

        _viewModel.TilesRemoved += async (transitions) =>
        {
            await ApplyRemoveTransitionsAsync(transitions);
        };

        _viewModel.TilesCreated += async (transitions) =>
        {
            await ApplyCreateTransitionsAsync(transitions);
        };
    }

    private async Task ApplyMoveTransitionsAsync(IEnumerable<TileTransition> transitions)
    {
        var trList = transitions.ToList();
        var moveTasks = new List<Task>();
        foreach (var mt in trList)
        {
            var view = FindTileView(mt.TileId);
            if (view != null)
            {
                double tx = (mt.ToX * tileSize) + ((mt.ToX + 1) * gapSize);
                double ty = (mt.ToY * tileSize) + ((mt.ToY + 1) * gapSize);

                moveTasks.Add(view.MoveToAsync(tx, ty));
            }
        }
        await Task.WhenAll(moveTasks);
    }

    private async Task ApplyRemoveTransitionsAsync(IEnumerable<TileTransition> transitions)
    {
        var trList = transitions.ToList();
        var removeTasks = new List<Task>();
        var idsToRemove = new List<int>();
        foreach (var rt in trList)
        {
            var view = FindTileView(rt.TileId);
            if (view is not null)
            {
                idsToRemove.Add(rt.TileId);
                removeTasks.Add(view.DisappearAsync(80));
            }
        }
        await Task.WhenAll(removeTasks);

        foreach (var id in idsToRemove)
        {
            GameGridLayout.Children.Remove(FindTileView(id));
            _tileViews.Remove(id);
        }
    }

    private async Task ApplyCreateTransitionsAsync(IEnumerable<TileTransition> transitions)
    {
        var trList = transitions.ToList();
        var createTasks = new List<Task>();
        foreach (var ct in trList)
        {
            var newVM = _viewModel.Tiles.FirstOrDefault(t => t.Id == ct.TileId);

            if (newVM is not null)
            {
                var tileView = new TileView { BindingContext = newVM };
                _tileViews[newVM.Id] = tileView;

                double tx = (ct.ToX * tileSize) + ((ct.ToX + 1) * gapSize);
                double ty = (ct.ToY * tileSize) + ((ct.ToY + 1) * gapSize);

                if (ct.Type == TileTransitionType.Respawn)
                {
                    double fx = (ct.FromX * tileSize) + ((ct.FromX + 1) * gapSize);
                    double fy = (ct.FromY * tileSize) + ((ct.FromY + 1) * gapSize);

                    AbsoluteLayout.SetLayoutBounds(tileView, new Rect(fx, fy, tileSize, tileSize));
                    GameGridLayout.Children.Add(tileView);

                    createTasks.Add(tileView.MoveToAsync(tx, ty));
                }
                else
                {
                    AbsoluteLayout.SetLayoutBounds(tileView, new Rect(tx, ty, tileSize, tileSize));
                    GameGridLayout.Children.Add(tileView);

                    if (ct.Type == TileTransitionType.Result)
                    {
                        createTasks.Add(tileView.PopAsync());
                    }
                    else
                    {
                        createTasks.Add(tileView.AppearAsync());
                    }
                }
            }
        }
        await Task.WhenAll(createTasks);
    }

    private TileView? FindTileView(int id)
    {
        return _tileViews.TryGetValue(id, out var view) ? view : null;
    }

    private void BuildTheBoard(int rows, int cols)
    {
        _targetWidth = (cols * tileSize) + ((cols + 1) * gapSize);
        _targetHeight = (rows * tileSize) + ((rows + 1) * gapSize);

        GameFrame.WidthRequest = _targetWidth;
        GameFrame.HeightRequest = _targetHeight;

        BackgroundGridLayout.Children.Clear();
        var emptyColor = (Application.Current?.Resources["EmptyCellColor"] as Color) ?? Colors.Gray;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var cell = new Border
                {
                    BackgroundColor = emptyColor,
                    StrokeShape = new RoundRectangle { CornerRadius = 8 },
                    WidthRequest = tileSize,
                    HeightRequest = tileSize
                };
                double x = (c * tileSize) + ((c + 1) * gapSize);
                double y = (r * tileSize) + ((r + 1) * gapSize);
                AbsoluteLayout.SetLayoutBounds(cell, new Rect(x, y, tileSize, tileSize));
                BackgroundGridLayout.Children.Add(cell);
            }
        }
    }

    private void OnFieldWrapperSizeChanged(object sender, EventArgs e)
    {
        if (_targetWidth <= 0 || _targetHeight <= 0) return;
        double finalScale = Math.Min((FieldBlockWrapper.Width - 40) / _targetWidth,
                                     (FieldBlockWrapper.Height - 40) / _targetHeight);
        GameFrame.Scale = finalScale;
    }

    private bool _isGestureHandled;
    private double _gestureStartX;
    private double _gestureStartY;

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        var viewModel = _viewModel;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _isGestureHandled = false;
                _gestureStartX = e.TotalX;
                _gestureStartY = e.TotalY;
                break;

            case GestureStatus.Running:
                if (_isGestureHandled) return;

                double dx = e.TotalX - _gestureStartX;
                double dy = e.TotalY - _gestureStartY;

                const double threshold = 10;

                if (Math.Abs(dx) > threshold || Math.Abs(dy) > threshold)
                {
                    _isGestureHandled = true;

                    string direction = "";
                    if (Math.Abs(dx) > Math.Abs(dy))
                    {
                        direction = (dx > 0) ? "Right" : "Left";
                    }
                    else
                    {
                        direction = (dy > 0) ? "Down" : "Up";
                    }

                    if (!string.IsNullOrEmpty(direction))
                    {
                        viewModel.MoveCommand.Execute(direction);
                    }
                }
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                _isGestureHandled = false;
                break;
        }
    }

    private void FullRedraw()
    {
        GameGridLayout.Children.Clear();
        _tileViews.Clear();

        foreach (var tileVM in _viewModel.Tiles) 
        {
            var tileView = new TileView { BindingContext = tileVM };
            _tileViews[tileVM.Id] = tileView;

            double x = (tileVM.Column * tileSize) + ((tileVM.Column + 1) * gapSize);
            double y = (tileVM.Row * tileSize) + ((tileVM.Row + 1) * gapSize);

            AbsoluteLayout.SetLayoutBounds(tileView, new Rect(x, y, tileSize, tileSize));
            GameGridLayout.Children.Add(tileView);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        BuildTheBoard(_viewModel.Rows, _viewModel.Columns);
        _viewModel.StartNewGame();
        FullRedraw();
    }
}