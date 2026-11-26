using LogHub.Composition;
using LogHub.Processing;
using Microsoft.Extensions.DependencyInjection;

// Композиционный корень: единственное место, где собирается контейнер и где есть доступ к диску.
if (args.Length < 1)
{
    Console.Error.WriteLine("Использование: LogHub <файл-лога> [файл-конфигурации]");
    return 1;
}

var logPath = args[0];
var configPath = args.Length > 1 ? args[1] : "loghub.config";

var configLines = File.Exists(configPath) ? File.ReadAllLines(configPath) : Array.Empty<string>();

using var provider = new ServiceCollection().AddLogHub().BuildLogHubProvider();

// Единственное обращение к IServiceProvider во всём приложении.
var app = provider.GetRequiredService<LogHubApp>();

var result = app.Run(configLines, File.ReadLines(logPath));

Console.WriteLine($"Записано: {result.Written}, пропущено: {result.Skipped}");
return 0;
