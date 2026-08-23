using Microsoft.Maui.Controls.Shapes;
using System.Windows.Input;

namespace Game2048.Maui.Views.Components;

public partial class DiamondButtonView : ContentView
{
    public static readonly BindableProperty ButtonColorProperty =
        BindableProperty.Create(nameof(ButtonColor), typeof(Brush), typeof(DiamondButtonView), Brush.DarkGray);

    public static readonly BindableProperty StrokeColorProperty =
        BindableProperty.Create(nameof(StrokeColor), typeof(Brush), typeof(DiamondButtonView), Brush.Transparent);

    public static readonly BindableProperty StrokeThicknessProperty =
        BindableProperty.Create(nameof(StrokeThickness), typeof(double), typeof(DiamondButtonView), 0.0);

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(DiamondButtonView));

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(DiamondButtonView));

    public static readonly BindableProperty IconSourceProperty =
        BindableProperty.Create(nameof(IconSource), typeof(ImageSource), typeof(DiamondButtonView));

    public static readonly BindableProperty ButtonSizeProperty =
        BindableProperty.Create(nameof(ButtonSize), typeof(double), typeof(DiamondButtonView), 76.0);

    public static readonly BindableProperty IconSizeProperty =
        BindableProperty.Create(nameof(IconSize), typeof(double), typeof(DiamondButtonView), 34.0);

    public static readonly BindableProperty CornerRadiusProperty =
    BindableProperty.Create(
        nameof(CornerRadius),
        typeof(Microsoft.Maui.CornerRadius),
        typeof(DiamondButtonView),
        new CornerRadius(18));

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public Brush ButtonColor { get => (Brush)GetValue(ButtonColorProperty); set => SetValue(ButtonColorProperty, value); }
    public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }
    public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }
    public ImageSource IconSource { get => (ImageSource)GetValue(IconSourceProperty); set => SetValue(IconSourceProperty, value); }
    public double ButtonSize { get => (double)GetValue(ButtonSizeProperty); set => SetValue(ButtonSizeProperty, value); }
    public double IconSize { get => (double)GetValue(IconSizeProperty); set => SetValue(IconSizeProperty, value); }
    public Brush StrokeColor { get => (Brush)GetValue(StrokeColorProperty); set => SetValue(StrokeColorProperty, value); }
    public double StrokeThickness { get => (double)GetValue(StrokeThicknessProperty); set => SetValue(StrokeThicknessProperty, value); }

    public DiamondButtonView()
    {
        InitializeComponent();
    }

    private async void OnTapped(object? sender, TappedEventArgs e)
    {
        await DiamondButtonBorder.ScaleToAsync(0.88, 60, Easing.CubicOut);
        await DiamondButtonBorder.ScaleToAsync(1.0, 90, Easing.CubicIn);
    }
}