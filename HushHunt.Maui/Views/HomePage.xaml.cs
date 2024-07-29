using HushHunt.Maui.ViewModels;
namespace HushHunt.Maui.Views;


public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
        BindingContext = new HomeViewModel();


    }

    private void Button_Clicked(object sender, EventArgs e)
    {
		Shell.Current.GoToAsync(nameof(GamePage));
    }
}