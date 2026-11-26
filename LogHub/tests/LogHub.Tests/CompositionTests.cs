using LogHub.Composition;
using LogHub.Config;
using LogHub.Processing;
using LogHub.Sinks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LogHub.Tests;

public class CompositionTests
{
    private static readonly string[] Lines =
    {
        "2026-09-14 09:00:01|Info|старт",
        "2026-09-14 09:00:05|Debug|детали",
        "мусор",
        "2026-09-14 09:02:40|Error|сбой"
    };

    private static ServiceProvider Build(Action<IServiceCollection>? replace = null)
    {
        var services = new ServiceCollection().AddLogHub();
        replace?.Invoke(services);
        return services.BuildLogHubProvider();
    }

    [Fact]
    public void Container_BuildsWithValidation()
    {
        using var provider = Build();

        Assert.NotNull(provider.GetRequiredService<LogHubApp>());
    }

    [Fact]
    public void Container_ConfigService_IsSingleton()
    {
        using var provider = Build();

        Assert.Same(provider.GetRequiredService<IConfigService>(), provider.GetRequiredService<IConfigService>());
    }

    [Fact]
    public void ConsoleProfile_WritesToConsole()
    {
        var console = new FakeConsole();
        using var provider = Build(s => s.AddSingleton<IConsoleOutput>(console));

        var result = provider.GetRequiredService<LogHubApp>().Run(new[] { "sink = console", "minSeverity = Info" }, Lines);

        Assert.Equal(2, result.Written);
        Assert.Equal(new[] { "2026-09-14 09:00:01 [Info] старт", "2026-09-14 09:02:40 [Error] сбой" }, console.Lines);
    }

    [Fact]
    public void FileProfile_WritesToWriterWithoutTouchingDisk()
    {
        var writers = new StringWriterFactory();
        using var provider = Build(s => s.AddSingleton<ITextWriterFactory>(writers));

        provider.GetRequiredService<LogHubApp>().Run(new[] { "sink = file", "path = out.txt" }, Lines);

        Assert.Equal("out.txt", writers.OpenedPath);
        Assert.Contains("[Error] сбой", writers.Writer.ToString());
    }

    [Fact]
    public void OwnConfigService_ReplacesDefaultOne()
    {
        var console = new FakeConsole();
        using var provider = Build(s =>
        {
            s.AddSingleton<IConsoleOutput>(console);
            s.AddSingleton<IConfigService>(new FixedConfigService("sink = console", "minSeverity = Error"));
        });

        // Строки конфигурации пустые: настройки приходят из подставленной реализации.
        var result = provider.GetRequiredService<LogHubApp>().Run(Array.Empty<string>(), Lines);

        Assert.Equal(1, result.Written);
        Assert.Single(console.Lines);
    }

    [Fact]
    public void UnknownSink_FailsWithClearMessage()
    {
        using var provider = Build();

        var error = Assert.Throws<NotSupportedException>(
            () => provider.GetRequiredService<LogHubApp>().Run(new[] { "sink = ftp" }, Lines));

        Assert.Equal("Приёмник 'ftp' не поддерживается", error.Message);
    }
}
