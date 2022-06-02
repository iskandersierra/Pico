namespace Pico.Tests;

public partial class EnumerableExtensionsTests
{
    [Fact]
    public void Consume()
    {
        var values = new List<int>(Enumerable.Range(1, 9));
        var valuesEnumerator = values.GetEnumerator();

        var enumeratorMock = new Mock<IEnumerator<int>>(MockBehavior.Strict);
        enumeratorMock.Setup(e => e.Dispose()).Callback(() => valuesEnumerator.Dispose());
        enumeratorMock.Setup(e => e.MoveNext()).Returns(() => valuesEnumerator.MoveNext());
        enumeratorMock.Setup(e => e.Current).Returns(() => valuesEnumerator.Current);

        var enumerableMock = new Mock<IEnumerable<int>>(MockBehavior.Strict);
        enumerableMock.Setup(e => e.GetEnumerator()).Returns(enumeratorMock.Object);

        enumerableMock.Object.Consume();

        enumerableMock.Verify(e => e.GetEnumerator(), Times.Once);
        enumeratorMock.Verify(e => e.Dispose(), Times.Once);
        enumeratorMock.Verify(e => e.MoveNext(), Times.Exactly(values.Count + 1));
        enumeratorMock.Verify(e => e.Current, Times.Exactly(values.Count));
    }

    [Fact]
    public async Task ConsumeAsync()
    {
        var values = new List<int>(Enumerable.Range(1, 9));
        var valuesEnumerator = values.GetEnumerator();

        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;

        var enumeratorMock = new Mock<IEnumerator<int>>(MockBehavior.Strict);
        enumeratorMock.Setup(e => e.Dispose()).Callback(() => valuesEnumerator.Dispose());
        enumeratorMock.Setup(e => e.MoveNext()).Returns(() => valuesEnumerator.MoveNext());
        enumeratorMock.Setup(e => e.Current).Returns(() => valuesEnumerator.Current);

        var enumerableMock = new Mock<IEnumerable<int>>(MockBehavior.Strict);
        enumerableMock.Setup(e => e.GetEnumerator()).Returns(enumeratorMock.Object);

        await enumerableMock.Object.ConsumeAsync(token);

        enumerableMock.Verify(e => e.GetEnumerator(), Times.Once);
        enumeratorMock.Verify(e => e.Dispose(), Times.Once);
        enumeratorMock.Verify(e => e.MoveNext(), Times.Exactly(values.Count + 1));
        enumeratorMock.Verify(e => e.Current, Times.Exactly(values.Count));
    }
}
