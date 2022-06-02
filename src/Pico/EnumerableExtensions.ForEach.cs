namespace Pico;

public static partial class EnumerableExtensions
{
    public static void ForEach<T>(
        this IEnumerable<T> source,
        Action<T> doAction)
    {
        foreach (var value in source)
        {
            doAction(value);
        }
    }

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
}
