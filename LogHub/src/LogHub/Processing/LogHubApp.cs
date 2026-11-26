using LogHub.Config;

namespace LogHub.Processing;

/// <summary>Прикладной сервис: единственная вещь, которую корень достаёт из контейнера.</summary>
public sealed class LogHubApp
{
    private readonly IConfigService _config;
    private readonly ILogProcessorFactory _processors;

    public LogHubApp(IConfigService config, ILogProcessorFactory processors)
    {
        _config = config;
        _processors = processors;
    }

    public ProcessingResult Run(IEnumerable<string> configLines, IEnumerable<string> logLines)
    {
        foreach (var pair in LogHubConfig.ParsePairs(configLines))
        {
            _config.SetValue(pair.Key, pair.Value);
        }

        using var processor = _processors.Create();
        return processor.Process(logLines);
    }
}
