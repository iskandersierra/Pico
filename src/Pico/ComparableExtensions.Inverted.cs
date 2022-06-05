namespace Pico;

partial class ComparableExtensions
{
    public static IComparer<T> Inverted<T>(this IComparer<T> comparer) =>
        new InvertedComparer<T>(comparer);

    private class InvertedComparer<T> : IComparer<T>
    {
        private readonly IComparer<T> innerComparer;

        public InvertedComparer(IComparer<T> innerComparer) =>
            this.innerComparer = innerComparer ??
                                 throw new ArgumentNullException(nameof(innerComparer));

        public int Compare(T? x, T? y) => -innerComparer.Compare(x, y);
    }
}
