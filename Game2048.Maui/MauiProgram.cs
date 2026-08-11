using Game2048.Core.Models;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Services;
using Game2048.Maui.ViewModels;
using Game2048.Maui.ViewModels.Items;
using Game2048.Maui.Views.Pages;
using Microsoft.Extensions.Logging;
using Game2048.Maui.Extensions;

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
                    fonts.AddFont("opensans_regular.ttf", "OpenSansRegular");
                    fonts.AddFont("opensans_semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("montserrat_bold.ttf", "MontBold");
                    fonts.AddFont("montserrat_semibold.ttf", "MontSemi");
                    fonts.AddFont("montserrat_regular.ttf", "MontReg");
                    fonts.AddFont("rubik_bold.ttf", "RubikBold");
                    fonts.AddFont("rubik_semibold.ttf", "RubikSemi");
                    fonts.AddFont("rubik_regular.ttf", "RubikRegular");
                    fonts.AddFont("fontawesome_solid.otf", "FontAwesome");
                });

            builder.Services.AddSingleton<AppShell>();

            builder.Services.AddSingleton<GameConfig>();
            builder.Services.AddSingleton<IGameModeService, GameModeService>();

            builder.Services.AddSingleton<IThemesManager, ThemesManager>();

            builder.Services.AddSingleton<ISaveService, SaveService>();
            builder.Services.AddSingleton<IProfileManager, ProfileManager>();
            builder.Services.AddSingleton<IStatisticsManager, StatisticsManager>();
            builder.Services.AddSingleton<IAchievementManager, AchievementManager>();
            builder.Services.AddSingleton<ISettingsManager, SettingsManager>();
            builder.Services.AddSingleton<IStoreManager, StoreManager>();

            builder.Services.AddSingleton<ThemeRegistry>();

            builder.Services.AddAchievementCheckers();

            builder.Services.AddTransient<LangSelectPage>();
            builder.Services.AddTransient<LangSelectViewModel>();
            builder.Services.AddTransient<GameView>();
            builder.Services.AddTransient<GameViewModel>();
            builder.Services.AddTransient<MainMenuViewModel>();
            builder.Services.AddTransient<MainMenuView>();
            builder.Services.AddTransient<GameModesView>();
            builder.Services.AddTransient<GameModesViewModel>();
            builder.Services.AddTransient<AchievementsViewModel>();
            builder.Services.AddTransient<AchievementsView>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<SettingsPageView>();
            builder.Services.AddTransient<ThemeSelectionViewModel>();
            builder.Services.AddTransient<ThemeSelectionView>();
            builder.Services.AddTransient<ThemePreviewViewModel>();
            builder.Services.AddTransient<ThemePreviewTileViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
