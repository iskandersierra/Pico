namespace Pico.Tests;

public partial class StringExtensionsTests
{
    private const string FullSampleText = "Sample text with two Text separators";

    // Spans

    [Theory]
    [InlineData(FullSampleText, "Text",
                StringComparison.InvariantCulture, false,
                "Sample text with two ")]
    [InlineData(FullSampleText, "TEXT",
                StringComparison.InvariantCulture, false,
                "")]
    [InlineData(FullSampleText, "Text",
                StringComparison.InvariantCultureIgnoreCase, false,
                "Sample ")]
    [InlineData(FullSampleText, "Text",
                StringComparison.InvariantCulture, true,
                "Sample text with two Text")]
    public void BeforeOrEmptySpan(
        string text, string separator,
        StringComparison comparison, bool includeSeparator,
        string expected) =>
        text.AsSpan()
            .BeforeOrEmpty(separator, comparison, includeSeparator)
            .ToString()
            .Should()
            .Be(expected);

    [Theory]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCulture, false,
        "Sample text with two ")]
    [InlineData(FullSampleText, "TEXT",
        StringComparison.InvariantCulture, false,
        FullSampleText)]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCultureIgnoreCase, false,
        "Sample ")]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCulture, true,
        "Sample text with two Text")]
    public void BeforeOrAllSpan(
        string text, string separator,
        StringComparison comparison, bool includeSeparator,
        string expected) =>
        text.AsSpan()
            .BeforeOrAll(separator, comparison, includeSeparator)
            .ToString()
            .Should()
            .Be(expected);

    [Theory]
    [InlineData(FullSampleText, "text",
                StringComparison.InvariantCulture, false,
                "Sample ")]
    [InlineData(FullSampleText, "TEXT",
                StringComparison.InvariantCulture, false,
                "")]
    [InlineData(FullSampleText, "text",
                StringComparison.InvariantCultureIgnoreCase, false,
                "Sample text with two ")]
    [InlineData(FullSampleText, "text",
                StringComparison.InvariantCulture, true,
                "Sample text")]
    public void BeforeLastOrEmptySpan(
        string text, string separator,
        StringComparison comparison, bool includeSeparator,
        string expected) =>
        text.AsSpan()
            .BeforeLastOrEmpty(separator, comparison, includeSeparator)
            .ToString()
            .Should()
            .Be(expected);

    [Theory]
    [InlineData(FullSampleText, "text",
                StringComparison.InvariantCulture, false,
                "Sample ")]
    [InlineData(FullSampleText, "TEXT",
                StringComparison.InvariantCulture, false,
                FullSampleText)]
    [InlineData(FullSampleText, "text",
                StringComparison.InvariantCultureIgnoreCase, false,
                "Sample text with two ")]
    [InlineData(FullSampleText, "text",
                StringComparison.InvariantCulture, true,
                "Sample text")]
    public void BeforeLastOrAllSpan(
        string text, string separator,
        StringComparison comparison, bool includeSeparator,
        string expected) =>
        text.AsSpan()
            .BeforeLastOrAll(separator, comparison, includeSeparator)
            .ToString()
            .Should()
            .Be(expected);

    [Theory]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCulture, false,
        " separators")]
    [InlineData(FullSampleText, "TEXT",
        StringComparison.InvariantCulture, false,
        "")]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCultureIgnoreCase, false,
        " with two Text separators")]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCulture, true,
        "Text separators")]
    public void AfterOrEmptySpan(
        string text, string separator,
        StringComparison comparison, bool includeSeparator,
        string expected) =>
        text.AsSpan()
            .AfterOrEmpty(separator, comparison, includeSeparator)
            .ToString()
            .Should()
            .Be(expected);

    [Theory]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCulture, false,
        " separators")]
    [InlineData(FullSampleText, "TEXT",
        StringComparison.InvariantCulture, false,
        FullSampleText)]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCultureIgnoreCase, false,
        " with two Text separators")]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCulture, true,
        "Text separators")]
    public void AfterOrAllSpan(
        string text, string separator,
        StringComparison comparison, bool includeSeparator,
        string expected) =>
        text.AsSpan()
            .AfterOrAll(separator, comparison, includeSeparator)
            .ToString()
            .Should()
            .Be(expected);

    [Theory]
    [InlineData(FullSampleText, "text",
        StringComparison.InvariantCulture, false,
        " with two Text separators")]
    [InlineData(FullSampleText, "TEXT",
        StringComparison.InvariantCulture, false,
        "")]
    [InlineData(FullSampleText, "text",
        StringComparison.InvariantCultureIgnoreCase, false,
        " separators")]
    [InlineData(FullSampleText, "text",
        StringComparison.InvariantCulture, true,
        "text with two Text separators")]
    public void AfterLastOrEmptySpan(
        string text, string separator,
        StringComparison comparison, bool includeSeparator,
        string expected) =>
        text.AsSpan()
            .AfterLastOrEmpty(separator, comparison, includeSeparator)
            .ToString()
            .Should()
            .Be(expected);

    [Theory]
    [InlineData(FullSampleText, "text",
        StringComparison.InvariantCulture, false,
        " with two Text separators")]
    [InlineData(FullSampleText, "TEXT",
        StringComparison.InvariantCulture, false,
        FullSampleText)]
    [InlineData(FullSampleText, "text",
        StringComparison.InvariantCultureIgnoreCase, false,
        " separators")]
    [InlineData(FullSampleText, "text",
        StringComparison.InvariantCulture, true,
        "text with two Text separators")]
    public void AfterLastOrAllSpan(
        string text, string separator,
        StringComparison comparison, bool includeSeparator,
        string expected) =>
        text.AsSpan()
            .AfterLastOrAll(separator, comparison, includeSeparator)
            .ToString()
            .Should()
            .Be(expected);

    // Strings

    [Theory]
    [InlineData(FullSampleText, "Text",
                StringComparison.InvariantCulture, false,
                "Sample text with two ")]
    [InlineData(FullSampleText, "TEXT",
                StringComparison.InvariantCulture, false,
                "")]
    [InlineData(FullSampleText, "Text",
                StringComparison.InvariantCultureIgnoreCase, false,
                "Sample ")]
    [InlineData(FullSampleText, "Text",
                StringComparison.InvariantCulture, true,
                "Sample text with two Text")]
    public void BeforeOrEmpty(
        string text, string separator,
        StringComparison comparison, bool includeSeparator,
        string expected) =>
        text.BeforeOrEmpty(separator, comparison, includeSeparator)
            .Should()
            .Be(expected);

    [Theory]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCulture, false,
        "Sample text with two ")]
    [InlineData(FullSampleText, "TEXT",
        StringComparison.InvariantCulture, false,
        FullSampleText)]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCultureIgnoreCase, false,
        "Sample ")]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCulture, true,
        "Sample text with two Text")]
    public void BeforeOrAll(
        string text, string separator,
        StringComparison comparison, bool includeSeparator,
        string expected) =>
        text.BeforeOrAll(separator, comparison, includeSeparator)
            .Should()
            .Be(expected);

    [Theory]
    [InlineData(FullSampleText, "text",
                StringComparison.InvariantCulture, false,
                "Sample ")]
    [InlineData(FullSampleText, "TEXT",
                StringComparison.InvariantCulture, false,
                "")]
    [InlineData(FullSampleText, "text",
                StringComparison.InvariantCultureIgnoreCase, false,
                "Sample text with two ")]
    [InlineData(FullSampleText, "text",
                StringComparison.InvariantCulture, true,
                "Sample text")]
    public void BeforeLastOrEmpty(
        string text, string separator,
        StringComparison comparison, bool includeSeparator,
        string expected) =>
        text.BeforeLastOrEmpty(separator, comparison, includeSeparator)
            .Should()
            .Be(expected);

    [Theory]
    [InlineData(FullSampleText, "text",
                StringComparison.InvariantCulture, false,
                "Sample ")]
    [InlineData(FullSampleText, "TEXT",
                StringComparison.InvariantCulture, false,
                FullSampleText)]
    [InlineData(FullSampleText, "text",
                StringComparison.InvariantCultureIgnoreCase, false,
                "Sample text with two ")]
    [InlineData(FullSampleText, "text",
                StringComparison.InvariantCulture, true,
                "Sample text")]
    public void BeforeLastOrAll(
        string text, string separator,
        StringComparison comparison, bool includeSeparator,
        string expected) =>
        text.BeforeLastOrAll(separator, comparison, includeSeparator)
            .Should()
            .Be(expected);

    [Theory]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCulture, false,
        " separators")]
    [InlineData(FullSampleText, "TEXT",
        StringComparison.InvariantCulture, false,
        "")]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCultureIgnoreCase, false,
        " with two Text separators")]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCulture, true,
        "Text separators")]
    public void AfterOrEmpty(
        string text, string separator,
        StringComparison comparison, bool includeSeparator,
        string expected) =>
        text.AfterOrEmpty(separator, comparison, includeSeparator)
            .Should()
            .Be(expected);

    [Theory]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCulture, false,
        " separators")]
    [InlineData(FullSampleText, "TEXT",
        StringComparison.InvariantCulture, false,
        FullSampleText)]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCultureIgnoreCase, false,
        " with two Text separators")]
    [InlineData(FullSampleText, "Text",
        StringComparison.InvariantCulture, true,
        "Text separators")]
    public void AfterOrAll(
        string text, string separator,
        StringComparison comparison, bool includeSeparator,
        string expected) =>
        text.AfterOrAll(separator, comparison, includeSeparator)
            .Should()
            .Be(expected);

    [Theory]
    [InlineData(FullSampleText, "text",
        StringComparison.InvariantCulture, false,
        " with two Text separators")]
    [InlineData(FullSampleText, "TEXT",
        StringComparison.InvariantCulture, false,
        "")]
    [InlineData(FullSampleText, "text",
        StringComparison.InvariantCultureIgnoreCase, false,
        " separators")]
    [InlineData(FullSampleText, "text",
        StringComparison.InvariantCulture, true,
        "text with two Text separators")]
    public void AfterLastOrEmpty(
        string text, string separator,
        StringComparison comparison, bool includeSeparator,
        string expected) =>
        text.AfterLastOrEmpty(separator, comparison, includeSeparator)
            .Should()
            .Be(expected);

    [Theory]
    [InlineData(FullSampleText, "text",
        StringComparison.InvariantCulture, false,
        " with two Text separators")]
    [InlineData(FullSampleText, "TEXT",
        StringComparison.InvariantCulture, false,
        FullSampleText)]
    [InlineData(FullSampleText, "text",
        StringComparison.InvariantCultureIgnoreCase, false,
        " separators")]
    [InlineData(FullSampleText, "text",
        StringComparison.InvariantCulture, true,
        "text with two Text separators")]
    public void AfterLastOrAll(
        string text, string separator,
        StringComparison comparison, bool includeSeparator,
        string expected) =>
        text.AfterLastOrAll(separator, comparison, includeSeparator)
            .Should()
            .Be(expected);
}
