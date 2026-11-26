using LogHub.Config;
using LogHub.Domain;
using LogHub.Sinks;

namespace LogHub.Tests;

/// <summary>Приёмник, который просто запоминает записи. Ни диска, ни консоли.</summary>
internal sealed class RecordingSink : ILogSink
{
    public List<(LogRecord Record, string Text)> Items { get; } = new();

    public bool Disposed { get; private set; }

    public void Write(LogRecord record, string text)
    {
        Items.Add((record, text));
    }

    public void Dispose()
    {
        Disposed = true;
    }
}

internal sealed class FakeConsole : IConsoleOutput
{
    public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;

    public List<string> Lines { get; } = new();

    public List<ConsoleColor> ColorsAtWrite { get; } = new();

    public void WriteLine(string text)
    {
        ColorsAtWrite.Add(ForegroundColor);
        Lines.Add(text);
    }
}

internal sealed class RecordingDbClient : IDbClient
{
    public List<string> Calls { get; } = new();

    public void Open() => Calls.Add("open");

    public void Execute(string sql, DateTime date, string severity, string text) =>
        Calls.Add($"execute:{severity}:{text}");

    public void Close() => Calls.Add("close");
}

/// <summary>Новый приёмник, добавленный «снаружи»: ни один файл LogHub для этого не менялся.</summary>
internal sealed class TelegramSinkFactory : ILogSinkFactory
{
    public string Name => "telegram";

    public ILogSink Create(LogHubConfig config) => new RecordingSink();
}

internal sealed class FixedDbClientFactory : IDbClientFactory
{
    private readonly IDbClient _client;

    public FixedDbClientFactory(IDbClient client)
    {
        _client = client;
    }

    public IDbClient Create(string connectionString) => _client;
}

internal sealed class StringWriterFactory : ITextWriterFactory
{
    public StringWriter Writer { get; } = new();

    public string? OpenedPath { get; private set; }

    public TextWriter Open(string path)
    {
        OpenedPath = path;
        return Writer;
    }
}

/// <summary>Своя реализация IConfigService: настройки заданы заранее, файл не читается.</summary>
internal sealed class FixedConfigService : IConfigService
{
    private readonly Dictionary<string, string> _values;

    public FixedConfigService(params string[] lines)
    {
        _values = new Dictionary<string, string>(LogHubConfig.ParsePairs(lines));
    }

    public string? GetValue(string key) => _values.TryGetValue(key, out var value) ? value : null;

    public void SetValue(string key, string value) => _values[key] = value;
}
