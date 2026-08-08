using Game2048.Maui.ViewModels;

namespace Game2048.Maui.Views.Components;

public partial class ScoreBoardView : ContentView
{
    public ScoreBoardView()
	{
		InitializeComponent();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is GameViewModel vm)
        {
            vm.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GameViewModel.Score))
        {
            AnimateScore();
        }
        
        if (e.PropertyName == nameof(GameViewModel.BestScore))
        {
            AnimateBestScore();
        }
    }

    private void AnimateScore()
    {
        Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(ActualScoreLabel);

        Dispatcher.Dispatch(async () =>
        {
            await ActualScoreLabel.ScaleToAsync(1.2, 100, Easing.CubicOut);
            await ActualScoreLabel.ScaleToAsync(1.0, 100, Easing.CubicIn);
        });
    }
    
    private void AnimateBestScore()
    {
        Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(BestScoreLabel);

        Dispatcher.Dispatch(async () =>
        {
            await BestScoreLabel.ScaleToAsync(1.2, 100, Easing.CubicOut);
            await BestScoreLabel.ScaleToAsync(1.0, 100, Easing.CubicIn);
        });
    }

    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        base.OnHandlerChanging(args);

        if (args.OldHandler != null && args.NewHandler == null)
        {
            if (BindingContext is GameViewModel vm)
            {
                vm.PropertyChanged -= OnViewModelPropertyChanged;
            }
        }
    }
}