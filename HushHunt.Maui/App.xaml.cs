
namespace HushHunt.Maui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window=base.CreateWindow(activationState);

            const int newWidth = 960;
            const int newHeight = 540;

     
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
