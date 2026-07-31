using Game2048.Maui.ViewModels;
using Game2048.Maui.ViewModels.Items;

namespace Game2048.Maui.Views.Pages;

public partial class ThemeSelectionView : ContentPage
{
    private readonly ThemeSelectionViewModel _viewModel;
	public ThemeSelectionView(ThemeSelectionViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
	}

	public async void OnBack(object sender, EventArgs e)
	{
        await Task.WhenAll
            (
                PageContainer.TranslateTo(Width, 0, 250, Easing.CubicIn),
                PageContainer.FadeTo(0, 250, Easing.Linear)
            );
        await Task.WhenAll
            (
                Shell.Current.GoToAsync("..", false)
            );
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        PageContainer.Opacity = 0;
        ThemeCollectionView.Opacity = 0;

        var selectedItem = _viewModel.SelectedThemeItem;
        ScrollThemeCollectionViewTo(selectedItem);

        await PageContainer.FadeTo(1, 600, Easing.CubicIn);
        await ThemeCollectionView.FadeTo(1.0, 400, Easing.CubicIn);
    }

    private void ScrollThemeCollectionViewTo(ThemeItemViewModel? item)
    {
        Dispatcher.Dispatch(() =>
        {
            if (item is not null)
            {
                ThemeCollectionView.ScrollTo(item, position: ScrollToPosition.Center, animate: false);
            }
        });
    }

    protected override bool OnBackButtonPressed() => true;
}