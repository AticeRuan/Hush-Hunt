
using Plugin.Maui.Audio;
namespace HushHunt.Maui
{
    public partial class App : Application
    {
        private readonly IAudioManager _audioManager;

        public App(IAudioManager audioManager)
        {
            _audioManager = audioManager;
            InitializeComponent();
            var appShell = new AppShell(_audioManager);
            Shell.SetBackgroundColor(appShell, Color.FromArgb("#c0a42e"));
            MainPage = appShell;
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            const int newWidth = 1152;
            const int newHeight = 648;

            window.Width = newWidth;
            window.Height = newHeight;

            window.MaximumHeight = newHeight;
            window.MinimumHeight = newHeight;
            window.MaximumWidth = newWidth;
            window.MinimumWidth = newWidth;

            return window;
        }
    }
}
