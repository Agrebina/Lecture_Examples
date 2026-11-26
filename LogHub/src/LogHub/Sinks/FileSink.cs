using LogHub.Config;
using LogHub.Domain;

namespace LogHub.Sinks;

public sealed class FileSink : ILogSink
{
    private readonly TextWriter _writer;

    public FileSink(TextWriter writer)
    {
        _writer = writer;
    }

    public void Write(LogRecord record, string text)
    {
        _writer.WriteLine(text);
        // Сброс на каждую запись оставлен как в исходном коде: поведение не меняем.
        _writer.Flush();
    }

    public void Dispose()
    {
        _writer.Dispose();
    }
}

/// <summary>Шов для доступа к диску: тесты подменяют его на StringWriter.</summary>
public interface ITextWriterFactory
{
    TextWriter Open(string path);
}

public sealed class FileWriterFactory : ITextWriterFactory
{
    public TextWriter Open(string path)
    {
        return new StreamWriter(path, append: true);
    }
}

public sealed class FileSinkFactory : ILogSinkFactory
{
    private readonly TimeProvider _time;
    private readonly ITextWriterFactory _writers;

    public FileSinkFactory(TimeProvider time, ITextWriterFactory writers)
    {
        _time = time;
        _writers = writers;
    }

    public string Name => "file";

    public ILogSink Create(LogHubConfig config)
    {
        // Ключ path необязателен: без него имя берётся по дате, как в ЛР 4.
        var path = config.Get("path", $"logs-{_time.GetLocalNow():yyyyMMdd}.txt");
        return new FileSink(_writers.Open(path));
    }
}
