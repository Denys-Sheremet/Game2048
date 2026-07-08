using System.Reflection;
using Game2048.Maui.Achievements.Interfaces;

namespace Game2048.Maui.Extensions;

public static class AchievementServiceCollectionExtensions
{
    public static IServiceCollection AddAchievementCheckers(this IServiceCollection services)
    {
        var assembly = typeof(App).Assembly;

        var checkerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract &&
                       (typeof(ISessionAchievementChecker).IsAssignableFrom(t) ||
                        typeof(IGlobalAchievementChecker).IsAssignableFrom(t) ||
                        typeof(ISpecialAchievementChecker).IsAssignableFrom(t)));

        foreach (var type in checkerTypes)
        {
            services.AddSingleton(type);

            if (typeof(ISessionAchievementChecker).IsAssignableFrom(type))
            {
                services.AddSingleton(typeof(ISessionAchievementChecker), sp => sp.GetRequiredService(type));
            }

            if (typeof(IGlobalAchievementChecker).IsAssignableFrom(type))
            {
                services.AddSingleton(typeof(IGlobalAchievementChecker), sp => sp.GetRequiredService(type));
            }

            if (typeof(ISpecialAchievementChecker).IsAssignableFrom(type))
            {
                services.AddSingleton(typeof(ISpecialAchievementChecker), sp => sp.GetRequiredService(type));
            }
        }

        return services;
    }
}
