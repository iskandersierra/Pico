using System.Runtime.CompilerServices;

namespace Pico;

public static partial class EnumerableExtensions
{
    public static IEnumerable<T> AsSingleton<T>(
        this T value)
    {
        yield return value;
    }

    public static IEnumerable<T> AsSingleton<T>(
        this Func<T> func)
    {
        yield return func();
    }

    public static async IAsyncEnumerable<T> AsSingletonAsync<T>(
        this T value)
    {
        await ValueTask.CompletedTask;
        yield return value;
    }

    public static async IAsyncEnumerable<T> AsSingletonAsync<T>(
        this Task<T> value)
    {
        yield return await value;
    }

    public static async IAsyncEnumerable<T> AsSingletonAsync<T>(
        this Func<T> value)
    {
        await ValueTask.CompletedTask;
        yield return value();
    }

    public static async IAsyncEnumerable<T> AsSingletonAsync<T>(
        this Func<Task<T>> value)
    {
        yield return await value();
    }

    public static async IAsyncEnumerable<T> AsSingletonAsync<T>(
        this Func<CancellationToken, Task<T>> value,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield return await value(cancellationToken);
    }
}
