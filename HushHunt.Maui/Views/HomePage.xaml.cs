
using HushHunt.Maui.Models;
using HushHunt.Maui.ViewModels;
using Plugin.Maui.Audio;
namespace HushHunt.Maui.Views;


public partial class HomePage : ContentPage
{



    public HomePage(IAudioManager audioManager)
	{
		InitializeComponent();
        BindingContext = new HomeViewModel();

    }


    private async void OnPointerEntered(object sender, PointerEventArgs e)
    {
        await StartButton.ScaleTo(1.2,200);
        SoundManager.Instance.PlaySound("swoosh.mp3");
        await StartButton.ScaleTo(1, 200);
      

    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        StartButton.Scale = 1.0; 
    }



}