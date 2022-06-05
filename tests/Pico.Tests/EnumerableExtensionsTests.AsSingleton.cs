namespace Pico.Tests;

public partial class EnumerableExtensionsTests
{
    [Fact]
    public void AsSingleton()
    {
        // GIVEN a value
        var value = 42;

        // WHEN a singleton collection is generated from value
        var singleton = value.AsSingleton();

        // THEN the singleton contains the value
        singleton.Should().BeEquivalentTo(new[] { value });
    }
}
