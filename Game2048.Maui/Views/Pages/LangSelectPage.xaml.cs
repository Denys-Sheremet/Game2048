using Game2048.Maui.ViewModels;

namespace Game2048.Maui.Views.Pages;

public partial class LangSelectPage : ContentPage
{
    private readonly LangSelectViewModel _viewModel;

	public LangSelectPage(LangSelectViewModel viewModel)
	{
		InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;

        _viewModel.IsLangSelected += ContinueToIntroAsync;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        PageContainer.Opacity = 0;
        await PageContainer.FadeTo(1.0, 200, Easing.CubicIn);
        
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _viewModel.IsLangSelected -= ContinueToIntroAsync;
    }

    private async void ContinueToIntroAsync()
    {
        PageContainer.Opacity = 1;
        await PageContainer.FadeTo(0.0, 200, Easing.CubicIn);

        await Task.WhenAll(
            Shell.Current.GoToAsync("///IntroPage", false)
        );
    }

    private async void OnBtnClicked(object sender, EventArgs e)
    {
		if (sender is ImageButton btn) 
		{
			await btn.ScaleTo(0.9, 100, Easing.CubicIn);
			await btn.ScaleTo(1.0, 100, Easing.CubicOut);
		}
    }
}