using LogHub.Config;
using LogHub.Formatting;
using LogHub.Parsing;
using LogHub.Sinks;

namespace LogHub.Processing;

/// <summary>
/// Процессор зависит от настроек, которые известны только после чтения конфигурации,
/// поэтому контейнер выдаёт не его, а фабрику. Создание по готовым зависимостям — работа фабрики.
/// </summary>
public interface ILogProcessorFactory
{
    LogProcessor Create();
}

public sealed class LogProcessorFactory : ILogProcessorFactory
{
    private readonly IConfigService _config;
    private readonly SinkRegistry _sinks;
    private readonly FormatterRegistry _formatters;

    public LogProcessorFactory(IConfigService config, SinkRegistry sinks, FormatterRegistry formatters)
    {
        _config = config;
        _sinks = sinks;
        _formatters = formatters;
    }

    public LogProcessor Create()
    {
        var config = new LogHubConfig(_config);
        var formatter = _formatters.Get(config.Format);

        return new LogProcessor(
            new DelimitedLogParser(config.Delimiter),
            formatter,
            _sinks.Create(config),
            config.MinSeverity);
    }
}
