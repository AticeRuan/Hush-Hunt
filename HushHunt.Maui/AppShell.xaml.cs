using HushHunt.Maui.Models;
using HushHunt.Maui.Views;
using Plugin.Maui.Audio;


namespace HushHunt.Maui
{
    public partial class AppShell : Shell
    {
       

        public AppShell(IAudioManager audioManager)
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(GamePage), typeof(GamePage));
            Routing.RegisterRoute(nameof(InfoPage), typeof(InfoPage));
            BindingContext = new AppShellViewModel();
                        
            SoundManager.Instance.Initialize(audioManager);
            SoundManager.Instance.PlayMusic("background.mp3", loop: true);
        }

        protected override void OnNavigated(ShellNavigatedEventArgs args)
        {
            base.OnNavigated(args);
            TitleLabel.Text = Current.CurrentPage.Title;
        }

        public void UpdateTitle(string title)
        {
            TitleLabel.Text = title;
        }
             

   

        private void Sound_Clicked(object sender, EventArgs e)
        {
            SoundManager.Instance.ToggleSound(!SoundManager.Instance.IsSoundEnabled);

            soundButton.IconImageSource = SoundManager.Instance.IsSoundEnabled ? "sound_icon.png" : "sound_icon_mute.png";




        }

        private void musicButton_Clicked(object sender, EventArgs e)
        {
            SoundManager.Instance.ToggleMusic(!SoundManager.Instance.IsMusicEnabled);
            musicButton.IconImageSource = SoundManager.Instance.IsMusicEnabled ? "music_icon.png" : "music_icon_mute.png";

        }
    }
}
