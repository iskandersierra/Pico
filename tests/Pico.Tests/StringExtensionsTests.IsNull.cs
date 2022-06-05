namespace Pico.Tests;

public partial class StringExtensionsTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData(" ", false)]
    [InlineData("\t", false)]
    [InlineData("\n", false)]
    [InlineData("\r", false)]
    [InlineData("xyz", false)]
    public void IsNullOrEmpty(string? value, bool expected) =>
        value.IsNullOrEmpty().Should().Be(expected);

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", true)]
    [InlineData("\t", true)]
    [InlineData("\n", true)]
    [InlineData("\r", true)]
    [InlineData("xyz", true)]
    public void IsNotNullOrEmpty(string? value, bool expected) =>
        value.IsNotNullOrEmpty().Should().Be(expected);

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData(" ", true)]
    [InlineData("\t", true)]
    [InlineData("\n", true)]
    [InlineData("\r", true)]
    [InlineData("xyz", false)]
    public void IsNullOrWhiteSpace(string? value, bool expected) =>
        value.IsNullOrWhiteSpace().Should().Be(expected);

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("\t", false)]
    [InlineData("\n", false)]
    [InlineData("\r", false)]
    [InlineData("xyz", true)]
    public void IsNotNullOrWhiteSpace(string? value, bool expected) =>
        value.IsNotNullOrWhiteSpace().Should().Be(expected);
}
