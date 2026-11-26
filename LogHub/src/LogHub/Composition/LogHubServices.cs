using LogHub.Config;
using LogHub.Formatting;
using LogHub.Parsing;
using LogHub.Processing;
using LogHub.Sinks;
using Microsoft.Extensions.DependencyInjection;

namespace LogHub.Composition;

/// <summary>
/// Композиционный корень (вместе с Program.cs): здесь решается, какие реализации
/// подставляются под интерфейсы и с каким временем жизни. Больше нигде new для сервисов не пишется.
/// </summary>
public static class LogHubServices
{
    public static IServiceCollection AddLogHub(this IServiceCollection services)
    {
        // Singleton: одни настройки на всё приложение, состояние живёт в контейнере, а не в static.
        services.AddSingleton<IConfigService, ConfigService>();

        services.AddSingleton<IConsoleOutput, SystemConsole>();
        services.AddSingleton<ITextWriterFactory, FileWriterFactory>();
        services.AddSingleton<IDbClientFactory, FakeDbClientFactory>();
        services.AddSingleton(TimeProvider.System);

        // Все фабрики приёмников и форматтеры, найденные в сборке.
        services.AddAll<ILogSinkFactory>();
        services.AddAll<ILogFormatter>();
        services.AddSingleton<SinkRegistry>();
        services.AddSingleton<FormatterRegistry>();

        // Transient: фабрика и приложение лёгкие и без состояния.
        services.AddTransient<ILogProcessorFactory, LogProcessorFactory>();
        services.AddTransient<LogHubApp>();

        return services;
    }

    public static ServiceProvider BuildLogHubProvider(this IServiceCollection services)
    {
        // Ошибки в регистрациях (например, captive dependency) вылетают при сборке, а не посреди работы.
        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });
    }
}
