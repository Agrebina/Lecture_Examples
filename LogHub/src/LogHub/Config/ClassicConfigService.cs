using System.Collections.Concurrent;

namespace LogHub.Config;

/// <summary>
/// Классический синглтон: закрытый конструктор и статический Instance().
/// Двойная проверка с lock по отдельному объекту (не lock(this)), поле volatile.
/// Схема из лекции по паттерну «Одиночка»; код написан заново.
/// </summary>
public sealed class ClassicConfigService : IConfigService
{
    private static readonly object Sync = new();
    private static volatile ClassicConfigService? _instance;

    private readonly ConcurrentDictionary<string, string> _values = new();

    private ClassicConfigService()
    {
    }

    public static ClassicConfigService Instance()
    {
        if (_instance is null)
        {
            lock (Sync)
            {
                _instance ??= new ClassicConfigService();
            }
        }

        return _instance;
    }

    public string? GetValue(string key)
    {
        return _values.TryGetValue(key, out var value) ? value : null;
    }

    public void SetValue(string key, string value)
    {
        _values[key] = value;
    }
}
