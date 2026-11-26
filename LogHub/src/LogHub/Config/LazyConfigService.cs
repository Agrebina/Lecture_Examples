using System.Collections.Concurrent;

namespace LogHub.Config;

/// <summary>Синглтон на Lazy&lt;T&gt;: создание отложено и потокобезопасно без ручного lock.</summary>
public sealed class LazyConfigService : IConfigService
{
    private static readonly Lazy<LazyConfigService> Holder = new(() => new LazyConfigService());

    private readonly ConcurrentDictionary<string, string> _values = new();

    private LazyConfigService()
    {
    }

    public static LazyConfigService Instance => Holder.Value;

    public string? GetValue(string key)
    {
        return _values.TryGetValue(key, out var value) ? value : null;
    }

    public void SetValue(string key, string value)
    {
        _values[key] = value;
    }
}
