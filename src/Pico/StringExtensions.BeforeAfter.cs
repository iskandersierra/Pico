using System.Text.RegularExpressions;

namespace Pico;

public delegate ReadOnlySpan<char> SeparatorNotFoundDelegate(
    ReadOnlySpan<char> text,
    ReadOnlySpan<char> separator);

public delegate ReadOnlySpan<char> SeparatorFoundDelegate(
    ReadOnlySpan<char> text,
    ReadOnlySpan<char> separator,
    int foundAtIndex);

partial class StringExtensions
{
    private static ReadOnlySpan<char> LookoutSeparator(
        this ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator,
        SeparatorFoundDelegate found,
        SeparatorNotFoundDelegate notFound,
        StringComparison comparisonType = StringComparison.InvariantCulture)
    {
        var index = text.IndexOf(separator, comparisonType);

        return index == -1
            ? notFound(text, separator)
            : found(text, separator, index);
    }

    private static ReadOnlySpan<char> LookoutLastSeparator(
        this ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator,
        SeparatorFoundDelegate found,
        SeparatorNotFoundDelegate notFound,
        StringComparison comparisonType = StringComparison.InvariantCulture)
    {
        var index = text.LastIndexOf(separator, comparisonType);

        return index == -1
            ? notFound(text, separator)
            : found(text, separator, index);
    }

    private static ReadOnlySpan<char> EmptySpanWhenNotFound(
        ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator) =>
        ReadOnlySpan<char>.Empty;

    private static ReadOnlySpan<char> AllSpanWhenNotFound(
        ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator) =>
        text;

    private static ReadOnlySpan<char> BeforeExcludedSeparatorWhenFound(
        ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator,
        int index) =>
        text[..index];

    private static ReadOnlySpan<char> BeforeIncludedSeparatorWhenFound(
        ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator,
        int index) =>
        text[..(index + separator.Length)];

    private static SeparatorFoundDelegate BeforeSeparatorWhenFound(
        bool includeSeparator) =>
        includeSeparator
            ? BeforeIncludedSeparatorWhenFound
            : BeforeExcludedSeparatorWhenFound;

    private static ReadOnlySpan<char> AfterExcludedSeparatorWhenFound(
        ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator,
        int index) =>
        text[(index + separator.Length)..];

    private static ReadOnlySpan<char> AfterIncludedSeparatorWhenFound(
        ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator,
        int index) =>
        text[index..];

    private static SeparatorFoundDelegate AfterSeparatorWhenFound(
        bool includeSeparator) =>
        includeSeparator
            ? AfterIncludedSeparatorWhenFound
            : AfterExcludedSeparatorWhenFound;

    /// <summary>
    /// Finds the text found before a given separator in a larger text.
    /// If the separator is not found, the empty text is returned.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="separator">The sequence of characters to search for in text</param>
    /// <param name="comparisonType">The type of character comparison</param>
    /// <param name="includeSeparator">Whether the separator must be included in the result.</param>
    /// <returns>
    /// The span of characters found before the given separator, if the
    /// separator is found. Otherwise, the empty text is returned.
    /// </returns>
    public static ReadOnlySpan<char> BeforeOrEmpty(
        this ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator,
        StringComparison comparisonType = StringComparison.InvariantCulture,
        bool includeSeparator = false) =>
        text.LookoutSeparator(
            separator,
            BeforeSeparatorWhenFound(includeSeparator),
            EmptySpanWhenNotFound,
            comparisonType);

    /// <summary>
    /// Finds the text found before a given separator in a larger text.
    /// If the separator is not found, the entire text is returned.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="separator">The sequence of characters to search for in text</param>
    /// <param name="comparisonType">The type of character comparison</param>
    /// <param name="includeSeparator">Whether the separator must be included in the result.</param>
    /// <returns>
    /// The span of characters found before the given separator, if the
    /// separator is found. Otherwise, the entire text is returned.
    /// </returns>
    public static ReadOnlySpan<char> BeforeOrAll(
        this ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator,
        StringComparison comparisonType = StringComparison.InvariantCulture,
        bool includeSeparator = false) =>
        text.LookoutSeparator(
            separator,
            BeforeSeparatorWhenFound(includeSeparator),
            AllSpanWhenNotFound,
            comparisonType);

    /// <summary>
    /// Finds the text found before the last occurrence of a given separator
    /// in a larger text. If the separator is not found, the empty text is returned.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="separator">The sequence of characters to search for in text</param>
    /// <param name="comparisonType">The type of character comparison</param>
    /// <param name="includeSeparator">Whether the separator must be included in the result.</param>
    /// <returns>
    /// The span of characters found before the last occurrence of the given
    /// separator, if the separator is found. Otherwise, the empty text is returned.
    /// </returns>
    public static ReadOnlySpan<char> BeforeLastOrEmpty(
        this ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator,
        StringComparison comparisonType = StringComparison.InvariantCulture,
        bool includeSeparator = false) =>
        text.LookoutLastSeparator(
            separator,
            BeforeSeparatorWhenFound(includeSeparator),
            EmptySpanWhenNotFound,
            comparisonType);

    /// <summary>
    /// Finds the text found before the last occurrence of a given separator
    /// in a larger text. If the separator is not found, the entire text is returned.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="separator">The sequence of characters to search for in text</param>
    /// <param name="comparisonType">The type of character comparison</param>
    /// <param name="includeSeparator">Whether the separator must be included in the result.</param>
    /// <returns>
    /// The span of characters found before the last occurrence of the given
    /// separator, if the separator is found. Otherwise, the entire text is returned.
    /// </returns>
    public static ReadOnlySpan<char> BeforeLastOrAll(
        this ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator,
        StringComparison comparisonType = StringComparison.InvariantCulture,
        bool includeSeparator = false) =>
        text.LookoutLastSeparator(
            separator,
            BeforeSeparatorWhenFound(includeSeparator),
            AllSpanWhenNotFound,
            comparisonType);

    
    /// <summary>
    /// Finds the text found after a given separator in a larger text.
    /// If the separator is not found, the empty text is returned.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="separator">The sequence of characters to search for in text</param>
    /// <param name="comparisonType">The type of character comparison</param>
    /// <param name="includeSeparator">Whether the separator must be included in the result.</param>
    /// <returns>
    /// The span of characters found after the given separator, if the
    /// separator is found. Otherwise, the empty text is returned.
    /// </returns>
    public static ReadOnlySpan<char> AfterOrEmpty(
        this ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator,
        StringComparison comparisonType = StringComparison.InvariantCulture,
        bool includeSeparator = false) =>
        text.LookoutSeparator(
            separator,
            AfterSeparatorWhenFound(includeSeparator),
            EmptySpanWhenNotFound,
            comparisonType);

    /// <summary>
    /// Finds the text found after a given separator in a larger text.
    /// If the separator is not found, the entire text is returned.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="separator">The sequence of characters to search for in text</param>
    /// <param name="comparisonType">The type of character comparison</param>
    /// <param name="includeSeparator">Whether the separator must be included in the result.</param>
    /// <returns>
    /// The span of characters found after the given separator, if the
    /// separator is found. Otherwise, the entire text is returned.
    /// </returns>
    public static ReadOnlySpan<char> AfterOrAll(
        this ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator,
        StringComparison comparisonType = StringComparison.InvariantCulture,
        bool includeSeparator = false) =>
        text.LookoutSeparator(
            separator,
            AfterSeparatorWhenFound(includeSeparator),
            AllSpanWhenNotFound,
            comparisonType);

    /// <summary>
    /// Finds the text found after the last occurrence of a given separator
    /// in a larger text. If the separator is not found, the empty text is returned.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="separator">The sequence of characters to search for in text</param>
    /// <param name="comparisonType">The type of character comparison</param>
    /// <param name="includeSeparator">Whether the separator must be included in the result.</param>
    /// <returns>
    /// The span of characters found after the last occurrence of the given
    /// separator, if the separator is found. Otherwise, the empty text is returned.
    /// </returns>
    public static ReadOnlySpan<char> AfterLastOrEmpty(
        this ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator,
        StringComparison comparisonType = StringComparison.InvariantCulture,
        bool includeSeparator = false) =>
        text.LookoutLastSeparator(
            separator,
            AfterSeparatorWhenFound(includeSeparator),
            EmptySpanWhenNotFound,
            comparisonType);

    /// <summary>
    /// Finds the text found after the last occurrence of a given separator
    /// in a larger text. If the separator is not found, the entire text is returned.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="separator">The sequence of characters to search for in text</param>
    /// <param name="comparisonType">The type of character comparison</param>
    /// <param name="includeSeparator">Whether the separator must be included in the result.</param>
    /// <returns>
    /// The span of characters found after the last occurrence of the given
    /// separator, if the separator is found. Otherwise, the entire text is returned.
    /// </returns>
    public static ReadOnlySpan<char> AfterLastOrAll(
        this ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator,
        StringComparison comparisonType = StringComparison.InvariantCulture,
        bool includeSeparator = false) =>
        text.LookoutLastSeparator(
            separator,
            AfterSeparatorWhenFound(includeSeparator),
            AllSpanWhenNotFound,
            comparisonType);


    /// <summary>
    /// Finds the text found before a given separator in a larger text.
    /// If the separator is not found, the empty text is returned.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="separator">The sequence of characters to search for in text</param>
    /// <param name="comparisonType">The type of character comparison</param>
    /// <param name="includeSeparator">Whether the separator must be included in the result.</param>
    /// <returns>
    /// The span of characters found before the given separator, if the
    /// separator is found. Otherwise, the empty text is returned.
    /// </returns>
    public static string BeforeOrEmpty(
        this string text,
        string separator,
        StringComparison comparisonType = StringComparison.InvariantCulture,
        bool includeSeparator = false) =>
        text.AsSpan()
            .BeforeOrEmpty(separator.AsSpan(), comparisonType, includeSeparator)
            .ToString();


    /// <summary>
    /// Finds the text found before a given separator in a larger text.
    /// If the separator is not found, the entire text is returned.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="separator">The sequence of characters to search for in text</param>
    /// <param name="comparisonType">The type of character comparison</param>
    /// <param name="includeSeparator">Whether the separator must be included in the result.</param>
    /// <returns>
    /// The span of characters found before the given separator, if the
    /// separator is found. Otherwise, the entire text is returned.
    /// </returns>
    public static string BeforeOrAll(
        this string text,
        string separator,
        StringComparison comparisonType = StringComparison.InvariantCulture,
        bool includeSeparator = false) =>
        text.AsSpan()
            .BeforeOrAll(separator.AsSpan(), comparisonType, includeSeparator)
            .ToString();

    /// <summary>
    /// Finds the text found before the last occurrence of a given separator
    /// in a larger text. If the separator is not found, the empty text is returned.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="separator">The sequence of characters to search for in text</param>
    /// <param name="comparisonType">The type of character comparison</param>
    /// <param name="includeSeparator">Whether the separator must be included in the result.</param>
    /// <returns>
    /// The span of characters found before the last occurrence of the given
    /// separator, if the separator is found. Otherwise, the empty text is returned.
    /// </returns>
    public static string BeforeLastOrEmpty(
        this string text,
        string separator,
        StringComparison comparisonType = StringComparison.InvariantCulture,
        bool includeSeparator = false) =>
        text.AsSpan()
            .BeforeLastOrEmpty(separator.AsSpan(), comparisonType, includeSeparator)
            .ToString();

    /// <summary>
    /// Finds the text found before the last occurrence of a given separator
    /// in a larger text. If the separator is not found, the entire text is returned.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="separator">The sequence of characters to search for in text</param>
    /// <param name="comparisonType">The type of character comparison</param>
    /// <param name="includeSeparator">Whether the separator must be included in the result.</param>
    /// <returns>
    /// The span of characters found before the last occurrence of the given
    /// separator, if the separator is found. Otherwise, the entire text is returned.
    /// </returns>
    public static string BeforeLastOrAll(
        this string text,
        string separator,
        StringComparison comparisonType = StringComparison.InvariantCulture,
        bool includeSeparator = false) =>
        text.AsSpan()
            .BeforeLastOrAll(separator.AsSpan(), comparisonType, includeSeparator)
            .ToString();


    /// <summary>
    /// Finds the text found after a given separator in a larger text.
    /// If the separator is not found, the empty text is returned.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="separator">The sequence of characters to search for in text</param>
    /// <param name="comparisonType">The type of character comparison</param>
    /// <param name="includeSeparator">Whether the separator must be included in the result.</param>
    /// <returns>
    /// The span of characters found after the given separator, if the
    /// separator is found. Otherwise, the empty text is returned.
    /// </returns>
    public static string AfterOrEmpty(
        this string text,
        string separator,
        StringComparison comparisonType = StringComparison.InvariantCulture,
        bool includeSeparator = false) =>
        text.AsSpan()
            .AfterOrEmpty(separator.AsSpan(), comparisonType, includeSeparator)
            .ToString();

    /// <summary>
    /// Finds the text found after a given separator in a larger text.
    /// If the separator is not found, the entire text is returned.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="separator">The sequence of characters to search for in text</param>
    /// <param name="comparisonType">The type of character comparison</param>
    /// <param name="includeSeparator">Whether the separator must be included in the result.</param>
    /// <returns>
    /// The span of characters found after the given separator, if the
    /// separator is found. Otherwise, the entire text is returned.
    /// </returns>
    public static string AfterOrAll(
        this string text,
        string separator,
        StringComparison comparisonType = StringComparison.InvariantCulture,
        bool includeSeparator = false) =>
        text.AsSpan()
            .AfterOrAll(separator.AsSpan(), comparisonType, includeSeparator)
            .ToString();

    /// <summary>
    /// Finds the text found after the last occurrence of a given separator
    /// in a larger text. If the separator is not found, the empty text is returned.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="separator">The sequence of characters to search for in text</param>
    /// <param name="comparisonType">The type of character comparison</param>
    /// <param name="includeSeparator">Whether the separator must be included in the result.</param>
    /// <returns>
    /// The span of characters found after the last occurrence of the given
    /// separator, if the separator is found. Otherwise, the empty text is returned.
    /// </returns>
    public static string AfterLastOrEmpty(
        this string text,
        string separator,
        StringComparison comparisonType = StringComparison.InvariantCulture,
        bool includeSeparator = false) =>
        text.AsSpan()
            .AfterLastOrEmpty(separator.AsSpan(), comparisonType, includeSeparator)
            .ToString();

    /// <summary>
    /// Finds the text found after the last occurrence of a given separator
    /// in a larger text. If the separator is not found, the entire text is returned.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="separator">The sequence of characters to search for in text</param>
    /// <param name="comparisonType">The type of character comparison</param>
    /// <param name="includeSeparator">Whether the separator must be included in the result.</param>
    /// <returns>
    /// The span of characters found after the last occurrence of the given
    /// separator, if the separator is found. Otherwise, the entire text is returned.
    /// </returns>
    public static string AfterLastOrAll(
        this string text,
        string separator,
        StringComparison comparisonType = StringComparison.InvariantCulture,
        bool includeSeparator = false) =>
        text.AsSpan()
            .AfterLastOrAll(separator.AsSpan(), comparisonType, includeSeparator)
            .ToString();


}
