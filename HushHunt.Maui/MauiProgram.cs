using Microsoft.Extensions.Logging;


using Plugin.Maui.Audio;
using HushHunt.Maui.Views;



namespace HushHunt.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Abaddon-Bold.ttf", "AbaddonBold");
                    fonts.AddFont("Abaddon-Light.ttf", "AbaddonLight");
                    fonts.AddFont("Pixeboy.ttf", "Pixeboy");
                    
                });

            builder.Services.AddSingleton(AudioManager.Current);
            builder.Services.AddTransient<GamePage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<GamePage>();
            builder.Services.AddTransient<AppShell>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            
            return builder.Build();
        }
    }
}
