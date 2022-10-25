using System.Collections;

namespace Pico;

public static partial class EnumerableExtensions
{
    public static bool IsNullOrEmpty<T>(
        [NotNullWhen(false)] this ICollection<T>? source) =>
        source is null || source.Count == 0;

    public static bool IsNotNullOrEmpty<T>(
        [NotNullWhen(true)] this ICollection<T>? source) =>
        !IsNullOrEmpty(source);


    public static bool IsNullOrEmpty<T>(
        [NotNullWhen(false)] this IReadOnlyCollection<T>? source) =>
        source is null || source.Count == 0;

    public static bool IsNotNullOrEmpty<T>(
        [NotNullWhen(true)] this IReadOnlyCollection<T>? source) =>
        !IsNullOrEmpty(source);


    public static bool IsNullOrEmpty(
        [NotNullWhen(false)] this ICollection? source) =>
        source is null || source.Count == 0;

    public static bool IsNotNullOrEmpty(
        [NotNullWhen(true)] this ICollection? source) =>
        !IsNullOrEmpty(source);


    public static bool IsNullOrEmpty<T>(
        [NotNullWhen(false)] this T[]? source) =>
        source is null || source.Length == 0;

    public static bool IsNotNullOrEmpty<T>(
        [NotNullWhen(true)] this T[]? source) =>
        !IsNullOrEmpty(source);

    public static bool IsNullOrEmpty<T>(
        [NotNullWhen(false)] this IEnumerable<T>? source) =>
        source switch
        {
            null => true,
            ICollection<T> collection => collection.IsNullOrEmpty(),
            IReadOnlyCollection<T> readOnlyCollection => readOnlyCollection.IsNullOrEmpty(),
            _ => !source.Any()
        };

    public static bool IsNotNullOrEmpty<T>(
        [NotNullWhen(true)] this IEnumerable<T>? source) =>
        !IsNullOrEmpty(source);
}
