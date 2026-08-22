namespace Game2048.Maui.Views.Pages;

public partial class HowToView : ContentPage
{
	public HowToView()
	{
		InitializeComponent();
	}

	private async Task GoBackAsync()
	{
        //animate
        await Shell.Current.GoToAsync("..", false);
    }

    protected override bool OnBackButtonPressed() 
	{
		Dispatcher.Dispatch(async () =>
		{
			await GoBackAsync();
		});
		return true;
	}
}