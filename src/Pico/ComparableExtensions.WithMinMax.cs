namespace Pico;

partial class ComparableExtensions
{
    public static T WithMinimum<T>(this T value, T minimum, IComparer<T> comparer) =>
        comparer.Compare(value, minimum) > 0 ? value : minimum;

    public static T WithMinimum<T>(this T value, T minimum)
        where T : IComparable<T> =>
        value.CompareTo(minimum) > 0 ? value : minimum;

    public static T WithMaximum<T>(this T value, T maximum, IComparer<T> comparer) =>
        comparer.Compare(value, maximum) < 0 ? value : maximum;

    public static T WithMaximum<T>(this T value, T maximum)
        where T : IComparable<T> =>
        value.CompareTo(maximum) < 0 ? value : maximum;

    public static T ClampedTo<T>(this T value, T minimum, T maximum, IComparer<T> comparer) =>
        value.WithMinimum(minimum, comparer).WithMaximum(maximum, comparer);

    public static T ClampedTo<T>(this T value, T minimum, T maximum)
        where T : IComparable<T> =>
        value.WithMinimum(minimum).WithMaximum(maximum);

    public static bool IsBetween<T>(this T value, T minimum, T maximum, IComparer<T> comparer) =>
        comparer.Compare(value, minimum) >= 0 && comparer.Compare(value, maximum) <= 0;

    public static bool IsBetween<T>(this T value, T minimum, T maximum)
        where T : IComparable<T> =>
        value.IsBetween(minimum, maximum, Comparer<T>.Default);

    public static bool IsBetweenExclusive<T>(this T value, T minimum, T maximum, IComparer<T> comparer) =>
        comparer.Compare(value, minimum) > 0 && comparer.Compare(value, maximum) < 0;

    public static bool IsBetweenExclusive<T>(this T value, T minimum, T maximum)
        where T : IComparable<T> =>
        value.IsBetweenExclusive(minimum, maximum, Comparer<T>.Default);
}
