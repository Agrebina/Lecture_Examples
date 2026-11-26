using System.Collections.Concurrent;

namespace LogHub.Config;

/// <summary>
/// Обычный класс с открытым конструктором. «Единственность» обеспечивает не он сам,
/// а регистрация в контейнере (AddSingleton). В тестах просто создаётся новый экземпляр.
/// </summary>
public sealed class ConfigService : IConfigService
{
    private readonly ConcurrentDictionary<string, string> _values = new();

    public string? GetValue(string key)
    {
        return _values.TryGetValue(key, out var value) ? value : null;
    }

    public void SetValue(string key, string value)
    {
        _values[key] = value;
    }
}
