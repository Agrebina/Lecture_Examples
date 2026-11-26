namespace LogHub.Config;

/// <summary>
/// Хранилище настроек «ключ — значение». Три реализации (классический синглтон,
/// синглтон на Lazy, обычный класс для контейнера) нужны для сравнения в ЛР 5.
/// </summary>
public interface IConfigService
{
    /// <summary>Значение по ключу или null, если ключ не задан.</summary>
    string? GetValue(string key);

    void SetValue(string key, string value);
}
