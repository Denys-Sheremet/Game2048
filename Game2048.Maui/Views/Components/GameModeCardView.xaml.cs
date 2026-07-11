using CommunityToolkit.Mvvm.Input;

namespace Game2048.Maui.Views.Components;

public partial class GameModeCardView : ContentView
{
	public static readonly BindableProperty CommandProperty =
		BindableProperty.Create(nameof(Command), typeof(IRelayCommand), typeof(GameModeCardView));

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(GameModeCardView));

    public IRelayCommand Command
    {
        get => (IRelayCommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public GameModeCardView()
	{
		InitializeComponent();
	}
}