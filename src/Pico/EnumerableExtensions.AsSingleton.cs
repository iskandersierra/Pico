namespace Pico;

public static partial class EnumerableExtensions
{
    public static IEnumerable<T> AsSingleton<T>(
        this T value)
    {
        yield return value;
    }
}
