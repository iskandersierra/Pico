namespace Pico;

public static partial class EnumerableExtensions
{
    public static bool AreDistinct<TValue, TKey>(
        this IEnumerable<TValue> source,
        Func<TValue, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null)
    {
        var set = new HashSet<TKey>(comparer ?? EqualityComparer<TKey>.Default);
        return source.All(value => set.Add(keySelector(value)));
    }

    public static bool AreDistinct<TValue>(
        this IEnumerable<TValue> source,
        IEqualityComparer<TValue>? comparer = null) =>
        source.AreDistinct(x => x, comparer);
}
