namespace Pico.Tests;

public partial class ComparableExtensionsTests
{
    [Theory]
    [InlineData(10, 20, 20)]
    [InlineData(30, 20, 30)]
    public void WithMinimum(int value, int minimum, int result) =>
        value.WithMinimum(minimum).Should().Be(result);

    [Theory]
    [InlineData(10, 20, 20)]
    [InlineData(30, 20, 30)]
    public void WithMinimumComparer(int value, int minimum, int result) =>
        value.WithMinimum(minimum, Comparer<int>.Default)
            .Should().Be(result);


    [Theory]
    [InlineData(10, 20, 10)]
    [InlineData(30, 20, 20)]
    public void WithMaximum(int value, int maximum, int result) =>
        value.WithMaximum(maximum).Should().Be(result);

    [Theory]
    [InlineData(10, 20, 10)]
    [InlineData(30, 20, 20)]
    public void WithMaximumComparer(int value, int maximum, int result) =>
        value.WithMaximum(maximum, Comparer<int>.Default)
            .Should().Be(result);


    [Theory]
    [InlineData(10, 20, 30, 20)]
    [InlineData(20, 20, 30, 20)]
    [InlineData(25, 20, 30, 25)]
    [InlineData(30, 20, 30, 30)]
    [InlineData(40, 20, 30, 30)]
    public void ClampedTo(int value, int minimum, int maximum, int result) =>
        value.ClampedTo(minimum, maximum).Should().Be(result);

    [Theory]
    [InlineData(10, 20, 30, 20)]
    [InlineData(20, 20, 30, 20)]
    [InlineData(25, 20, 30, 25)]
    [InlineData(30, 20, 30, 30)]
    [InlineData(40, 20, 30, 30)]
    public void ClampedToComparer(int value, int minimum, int maximum, int result) =>
        value.ClampedTo(minimum, maximum, Comparer<int>.Default)
            .Should().Be(result);
}
