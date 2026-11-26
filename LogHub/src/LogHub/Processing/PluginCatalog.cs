using Microsoft.Extensions.DependencyInjection;

namespace LogHub.Processing;

/// <summary>
/// Находит в сборке все классы, реализующие интерфейс. С ЛР 5 каталог сам ничего не создаёт:
/// он только регистрирует найденные типы в контейнере, а объекты строит контейнер.
/// Новый приёмник или формат по-прежнему добавляется одним новым файлом.
/// </summary>
public static class PluginCatalog
{
    public static IReadOnlyList<Type> FindTypes<T>() where T : class
    {
        return typeof(T).Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(T).IsAssignableFrom(t))
            .ToList();
    }

    public static IServiceCollection AddAll<T>(this IServiceCollection services) where T : class
    {
        foreach (var type in FindTypes<T>())
        {
            services.AddSingleton(typeof(T), type);
        }

        return services;
    }
}
