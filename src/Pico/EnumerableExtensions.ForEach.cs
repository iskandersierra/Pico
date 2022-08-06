namespace Pico;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// This extension method is used to iterate over a collection and
    /// execute a delegate for each item, until false is returned.
    /// <param name="source">The collection to iterate over.</param>
    /// <param name="doAndContinue">is the delegate to execute for each item.
    /// If it returns false for an item, the iteration stops, otherwise,
    /// it continues to the next items.</param>
    /// </summary>
    public static void ForEach<T>(
        this IEnumerable<T> source,
        Func<T, bool> doAndContinue)
    {
        foreach (var value in source)
        {
            if (!doAndContinue(value))
            {
                break;
            }
        }
    }

    /// <summary>
    /// This extension method is used to iterate over a collection and
    /// execute a delegate for each item.
    /// <param name="source">The collection to iterate over.</param>
    /// <param name="doAction">is the async delegate to execute for each item.</param>
    /// </summary>
    public static void ForEach<T>(
        this IEnumerable<T> source,
        Action<T> doAction)
    {
        foreach (var value in source)
        {
            doAction(value);
        }
    }

    /// <summary>
    /// This extension method is used to iterate over a collection and
    /// execute an async delegate for each item.
    /// <param name="source"> is the collection to iterate over.</param>
    /// <param name="doAndContinue">is the async delegate to execute for each item.
    /// If it returns false for an item, the iteration stops, otherwise,
    /// it continues to the next items.</param>
    /// <param name="cancellationToken"> is the cancellation token to use.</param>
    /// </summary>
    public static async Task ForEachAsync<T>(
        this IEnumerable<T> source,
        Func<T, CancellationToken, Task<bool>> doAndContinue,
        CancellationToken cancellationToken = default)
    {
        foreach (var value in source)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!await doAndContinue(value, cancellationToken))
            {
                break;
            }
        }
    }

    /// <summary>
    /// This extension method is used to iterate over a collection and
    /// execute an async delegate for each item.
    /// <param name="source"> is the collection to iterate over.</param>
    /// <param name="doAndContinue">is the async delegate to execute for each item.
    /// If it returns false for an item, the iteration stops, otherwise,
    /// it continues to the next items.</param>
    /// <param name="cancellationToken"> is the cancellation token to use.</param>
    /// </summary>
    public static async Task ForEachAsync<T>(
        this IEnumerable<T> source,
        Func<T, Task<bool>> doAndContinue,
        CancellationToken cancellationToken = default)
    {
        foreach (var value in source)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!await doAndContinue(value))
            {
                break;
            }
        }
    }

    /// <summary>
    /// This extension method is used to iterate over a collection and
    /// execute an async delegate for each item.
    /// <param name="source"> is the collection to iterate over.</param>
    /// <param name="doAction"> is the async delegate to execute for each item.</param>
    /// <param name="cancellationToken"> is the cancellation token to use.</param>
    /// </summary>
    public static async Task ForEachAsync<T>(
        this IEnumerable<T> source,
        Func<T, CancellationToken, Task> doAction,
        CancellationToken cancellationToken = default)
    {
        foreach (var value in source)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await doAction(value, cancellationToken);
        }
    }

    /// <summary>
    /// This extension method is used to iterate over a collection and
    /// execute an async delegate for each item.
    /// <param name="source"> is the collection to iterate over.</param>
    /// <param name="doAction"> is the async delegate to execute for each item.</param>
    /// <param name="cancellationToken"> is the cancellation token to use.</param>
    /// </summary>
    public static async Task ForEachAsync<T>(
        this IEnumerable<T> source,
        Func<T, Task> doAction,
        CancellationToken cancellationToken = default)
    {
        foreach (var value in source)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await doAction(value);
        }
    }

    /// <summary>
    /// This extension method is used to iterate over a collection and
    /// execute an async delegate for each item.
    /// <param name="source"> is the collection to iterate over.</param>
    /// <param name="doAndContinue">is the async delegate to execute for each item.
    /// If it returns false for an item, the iteration stops, otherwise,
    /// it continues to the next items.</param>
    /// <param name="cancellationToken"> is the cancellation token to use.</param>
    /// </summary>
    public static async Task ForEachAsync<T>(
        this IAsyncEnumerable<T> source,
        Func<T, CancellationToken, Task<bool>> doAndContinue,
        CancellationToken cancellationToken = default)
    {
        await foreach (var value in source.WithCancellation(cancellationToken))
        {
            if (!await doAndContinue(value, cancellationToken))
            {
                break;
            }
        }
    }

    /// <summary>
    /// This extension method is used to iterate over a collection and
    /// execute an async delegate for each item.
    /// <param name="source"> is the collection to iterate over.</param>
    /// <param name="doAndContinue">is the async delegate to execute for each item.
    /// If it returns false for an item, the iteration stops, otherwise,
    /// it continues to the next items.</param>
    /// <param name="cancellationToken"> is the cancellation token to use.</param>
    /// </summary>
    public static async Task ForEachAsync<T>(
        this IAsyncEnumerable<T> source,
        Func<T, Task<bool>> doAndContinue,
        CancellationToken cancellationToken = default)
    {
        await foreach (var value in source.WithCancellation(cancellationToken))
        {
            if (!await doAndContinue(value))
            {
                break;
            }
        }
    }

    /// <summary>
    /// This extension method is used to iterate over a collection and
    /// execute an async delegate for each item.
    /// <param name="source"> is the collection to iterate over.</param>
    /// <param name="doAction"> is the async delegate to execute for each item.</param>
    /// <param name="cancellationToken"> is the cancellation token to use.</param>
    /// </summary>
    public static async Task ForEachAsync<T>(
        this IAsyncEnumerable<T> source,
        Func<T, CancellationToken, Task> doAction,
        CancellationToken cancellationToken = default)
    {
        await foreach (var value in source.WithCancellation(cancellationToken))
        {
            await doAction(value, cancellationToken);
        }
    }

    /// <summary>
    /// This extension method is used to iterate over a collection and
    /// execute an async delegate for each item.
    /// <param name="source"> is the collection to iterate over.</param>
    /// <param name="doAction"> is the async delegate to execute for each item.</param>
    /// <param name="cancellationToken"> is the cancellation token to use.</param>
    /// </summary>
    public static async Task ForEachAsync<T>(
        this IAsyncEnumerable<T> source,
        Func<T, Task> doAction,
        CancellationToken cancellationToken = default)
    {
        await foreach (var value in source.WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            await doAction(value);
        }
    }
}
