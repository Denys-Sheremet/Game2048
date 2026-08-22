using Game2048.Core.DTOs;
using Game2048.Core.Enums;
using Game2048.Maui.Achievements.Services;
using Game2048.Maui.Enums;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Services;
using Game2048.Maui.ViewModels;
using Game2048.Maui.Views.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Shapes;

namespace Game2048.Maui.Views.Pages;

public partial class GameView : ContentPage
{
    private double _targetWidth;
    private double _targetHeight;

    private bool _isPageLoaded = false;

    private readonly GameViewModel _viewModel;
    private readonly IAchievementManager _achievementManager;
    private readonly Dictionary<int, TileView> _tileViews = new();
    private readonly ILogger<GameView> _logger;

    public GameView(GameViewModel viewModel, 
                    IAchievementManager achievementManager,
                    ILogger<GameView> logger)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;

        _achievementManager = achievementManager;
        _logger = logger;

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

        GameOverOverlay.GoToMenuRequested += OnGoToMenu;
        VictoryOverlay.GoToMenuRequested += OnGoToMenu;

        Loaded += OnPageLoaded;
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
                double tx = LayoutConstants.GetCoordinate(mt.ToX);
                double ty = LayoutConstants.GetCoordinate(mt.ToY);

                moveTasks.Add(view.MoveToAsync(tx, ty));
            }
        }
        await Task.WhenAll(moveTasks);
    }

    private async Task ApplyRemoveTransitionsAsync(IEnumerable<TileTransition> transitions)
    {
        var viewsToRemove = transitions
            .Select(rt => new {Id = rt.TileId, View = FindTileView(rt.TileId)})
            .Where(x => x.View != null)
            .ToList();

        var removeTasks = viewsToRemove.Select(x => x.View!.DisappearAsync());

        await Task.WhenAll(removeTasks);

        foreach (var item in viewsToRemove)
        {
            GameGridLayout.Children.Remove(item.View);

            if (_tileViews.TryGetValue(item.Id, out var currentView) && currentView == item.View)
            {
                _tileViews.Remove(item.Id);
            }
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

                double tx = LayoutConstants.GetCoordinate(ct.ToX);
                double ty = LayoutConstants.GetCoordinate(ct.ToY);
                double size = LayoutConstants.TileSize;

                if (ct.Type == TileTransitionType.Respawn)
                {
                    double fx = LayoutConstants.GetCoordinate(ct.FromX);
                    double fy = LayoutConstants.GetCoordinate(ct.FromY);

                    AbsoluteLayout.SetLayoutBounds(tileView, new Rect(fx, fy, size, size));
                    GameGridLayout.Children.Add(tileView);

                    createTasks.Add(tileView.RespawnToAsync(tx, ty)); //150?
                }
                else
                {
                    AbsoluteLayout.SetLayoutBounds(tileView, new Rect(tx, ty, size, size));
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
        _targetWidth = LayoutConstants.GetBoardSize(cols);
        _targetHeight = LayoutConstants.GetBoardSize(rows);

        GameFrame.WidthRequest = _targetWidth;
        GameFrame.HeightRequest = _targetHeight;

        BackgroundGridLayout.Children.Clear();
        var emptyColor = (Application.Current?.Resources["EmptyCellColor"] as Color) ?? Colors.Gray;

        double size = LayoutConstants.TileSize;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var cell = new Border
                {
                    BackgroundColor = emptyColor,
                    StrokeShape = new RoundRectangle { CornerRadius = 8 },
                    WidthRequest = size,
                    HeightRequest = size
                };
                double x = LayoutConstants.GetCoordinate(c);
                double y = LayoutConstants.GetCoordinate(r);
                AbsoluteLayout.SetLayoutBounds(cell, new Rect(x, y, size, size));
                BackgroundGridLayout.Children.Add(cell);
            }
        }
    }

    private void OnFieldWrapperSizeChanged(object sender, EventArgs e)
    {
        GameFrame.Scale = LayoutConstants.CalculateScale(
            FieldBlockWrapper.Width, 
            FieldBlockWrapper.Height, 
            _targetWidth, 
            _targetHeight
        );
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

        double size = LayoutConstants.TileSize;

        foreach (var tileVM in _viewModel.Tiles) 
        {
            var tileView = new TileView { BindingContext = tileVM };
            _tileViews[tileVM.Id] = tileView;

            double x = LayoutConstants.GetCoordinate(tileVM.Column);
            double y = LayoutConstants.GetCoordinate(tileVM.Row);

            AbsoluteLayout.SetLayoutBounds(tileView, new Rect(x, y, size, size));
            GameGridLayout.Children.Add(tileView);
        }
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        await AnimatePageAppearing();

        _isPageLoaded = true;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        BuildTheBoard(_viewModel.Rows, _viewModel.Columns);
        FullRedraw();

        if (_isPageLoaded)
        {
            await AnimatePageAppearing();
        }

        _viewModel.OnVictory += HandleOnVictory;
        _viewModel.OnGameOver += HandleOnGameOver;
        _viewModel.OnRestart += HandleRestart;
        _viewModel.OnSettings += HandleOnSettings;
        _achievementManager.AchievementUnlocked += OnNewAchievementUnlocked;
    }

    private async Task AnimatePageAppearing()
    {
        PageGrid.TranslationX = -200;
        PageGrid.Opacity = 0;
        PageGrid.IsVisible = true;
        await Task.Yield();

        await Task.WhenAll(
            PageGrid.TranslateToAsync(0, 0, 300, Easing.CubicOut),
            PageGrid.FadeToAsync(1, 300, Easing.CubicOut)
        );
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _viewModel.OnVictory -= HandleOnVictory;
        _viewModel.OnGameOver -= HandleOnGameOver;
        _viewModel.OnRestart -= HandleRestart;
        _viewModel.OnSettings -= HandleOnSettings;
        _achievementManager.AchievementUnlocked -= OnNewAchievementUnlocked;
    }

    private void OnNewAchievementUnlocked(AchievementType type)
    {
        string title = AchievementDataParser.GetTitle(type);
        string desc = AchievementDataParser.GetDesc(type);
        string imageName = AchievementDataParser.GetImageName(type);

        Dispatcher.Dispatch(async () =>
        {
            try
            {
                await AchievementToast.ShowAsync(title, desc, imageName);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error occurred during AchievementToast show");
            }
        });
    }

    private async Task PageDisappearToAsync(double translationX)
    {
        await Task.WhenAll(
            PageGrid.TranslateToAsync(translationX, 0, 300, Easing.CubicIn),
            PageGrid.FadeToAsync(0, 300, Easing.CubicIn)
        );
    }

    private async void OnGoToMenu(object? sender, EventArgs e)
    {
        await _viewModel.OnGoToMenu();

        await PageDisappearToAsync(200);

        await Shell.Current.GoToAsync("///MainMenuPage", false);
    }

    private async void OnBackToMenu(object? sender, EventArgs e)
    {
        await _viewModel.OnBackToMenu();

        await PageDisappearToAsync(200);

        await Shell.Current.GoToAsync("///MainMenuPage", false);
    }


    private async void HandleOnVictory()
    {
        VictoryOverlay.TranslationY = 800;
        VictoryOverlay.Opacity = 0;

        await Task.WhenAll(
            VictoryOverlay.FadeToAsync(1, 400),
            VictoryOverlay.TranslateToAsync(0, 0, 800, Easing.BounceOut)
        );
    }

    private async void HandleOnGameOver()
    {
        GameOverOverlay.Scale = 0.0;
        GameOverOverlay.Opacity = 0.0;

        await Task.WhenAll(
            GameOverOverlay.FadeToAsync(1, 200),
            GameOverOverlay.ScaleToAsync(1.1, 300, Easing.CubicOut)
        );

        await GameOverOverlay.ScaleToAsync(1.0, 100, Easing.CubicIn);
    }

    private async void HandleRestart()
    {
        await GameGridLayout.FadeToAsync(0.0, 150);

        FullRedraw();

        await GameGridLayout.FadeToAsync(1.0, 150);
    }

    private async void HandleOnSettings()
    {
        Settings.Opacity = 0;
        Settings.Scale = 0.0;

        await Task.WhenAll(
            Settings.FadeToAsync(1, 200),
            Settings.ScaleToAsync(1.1, 300, Easing.CubicIn)
        );
        await Settings.ScaleToAsync(1.0, 100, Easing.CubicOut);
    }

    protected override bool OnBackButtonPressed()
    {
        if (BindingContext is GameViewModel vm)
        {
            if (vm.IsActiveGame)
            {
                vm.OpenSettingsCommand.Execute(null);
            }
            else if (vm.IsSettings)
            {
                vm.CloseSettingsCommand.Execute(null);
            }
        }
        return true;
    }

    public async Task AnimateAndNavigateToThemesAsync()
    {
        await PageDisappearToAsync(-200);

        if (BindingContext is GameViewModel vm)
        {
            if (vm.IsSettings)
            {
                vm.CloseSettingsCommand.Execute(null);
            }
        }

        await Shell.Current.GoToAsync(nameof(ThemeSelectionView), false);
    }
}