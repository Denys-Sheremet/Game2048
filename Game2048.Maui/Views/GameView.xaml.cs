using Game2048.Maui.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace Game2048.Maui.Views;

public partial class GameView : ContentPage
{
    private double _targetWidth;
    private double _targetHeight;
    private GameViewModel _viewModel;

    public GameView()
    {
        InitializeComponent();

        _viewModel = new GameViewModel(4, 4);
        BindingContext = _viewModel;

        BuildTheBoard(_viewModel.Rows, _viewModel.Columns);
    }

    private void BuildTheBoard(int rows, int cols)
    {
        const double tileSize = 100;
        const double gapSize = 10;

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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.StartNewGame();
    }
}