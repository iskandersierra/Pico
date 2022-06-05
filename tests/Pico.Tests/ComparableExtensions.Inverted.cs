namespace Pico.Tests;

public partial class ComparableExtensionsTests
{
    [Property]
    public void Inverted(int value1, int value2) =>
        Comparer<int>.Default.Compare(value1, value2)
            .Should()
            .Be(-Comparer<int>.Default.Inverted().Compare(value1, value2));
}
