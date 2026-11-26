using Xunit.Abstractions;
using Xunit.Sdk;

namespace LogHub.Tests;

/// <summary>
/// Запускает тесты класса по алфавиту имён. Нужен, чтобы зависимость от порядка
/// в ClassicSingletonTests воспроизводилась всегда, а не «иногда».
/// </summary>
public sealed class AlphabeticalOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        return testCases.OrderBy(t => t.TestMethod.Method.Name, StringComparer.Ordinal);
    }
}
