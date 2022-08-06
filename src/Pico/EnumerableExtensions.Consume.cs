namespace Pico;

public static partial class EnumerableExtensions
{
    public static void Consume<T>(
        this IEnumerable<T> source) =>
        source.ForEach(_ => { });

    public static async Task ConsumeAsync<T>(
        this IEnumerable<T> source,
        CancellationToken cancellationToken = default) =>
        await source.ForEachAsync(_ => Task.CompletedTask, cancellationToken);

    public static async Task ConsumeAsync<T>(
        this IAsyncEnumerable<T> source,
        CancellationToken cancellationToken = default) =>
        await source.ForEachAsync(_ => Task.CompletedTask, cancellationToken);
}
