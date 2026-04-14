using Microsoft.Extensions.Logging;

namespace Game2048.Maui
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
                    fonts.AddFont("Montserrat-Bold.ttf", "MontBold");
                    fonts.AddFont("Montserrat-SemiBold.ttf", "MontSemi");
                    fonts.AddFont("Montserrat-Regular.ttf", "MontReg");
                    fonts.AddFont("Rubik-Bold.ttf", "RubikBold");
                    fonts.AddFont("Rubik-Medium.ttf", "RubikMedium");
                    fonts.AddFont("Rubik-Regular.ttf", "RubikRegular");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
