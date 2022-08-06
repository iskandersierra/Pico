using System.Runtime.CompilerServices;

namespace Pico;

public static partial class EnumerableExtensions
{
    public static IEnumerable<T> Tee<T>(
        this IEnumerable<T> source,
        Action<T> action) =>
        source.Select(value =>
            {
                action(value);
                return value;
            });

    public static async IAsyncEnumerable<T> TeeAsync<T>(
        this IEnumerable<T> source,
        Func<T, Task> action,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var value in source)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await action(value);
            yield return value;
        }
    }

    public static async IAsyncEnumerable<T> TeeAsync<T>(
        this IEnumerable<T> source,
        Func<T, CancellationToken, Task> action,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var value in source)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await action(value, cancellationToken);
            yield return value;
        }
    }

    public static async IAsyncEnumerable<T> TeeAsync<T>(
        this IAsyncEnumerable<T> source,
        Func<T, Task> action,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var value in source.WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            await action(value);
            yield return value;
        }
    }

    public static async IAsyncEnumerable<T> TeeAsync<T>(
        this IAsyncEnumerable<T> source,
        Func<T, CancellationToken, Task> action,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var value in source.WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            await action(value, cancellationToken);
            yield return value;
        }
    }
}
