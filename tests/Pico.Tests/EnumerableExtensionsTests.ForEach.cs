namespace Pico.Tests;

public partial class EnumerableExtensionsTests
{
    [Fact]
    public void ForEach_While()
    {
        // GIVEN a collection size
        var count = 9;

        // And a maximum value valid for the delegate
        var maxValid = 5;

        // And a collection with count elements
        var values = Enumerable.Range(1, count);

        // And a delegate to receive items and reject invalid values
        var doAndContinueMock = new Mock<Func<int, bool>>(MockBehavior.Strict);
        doAndContinueMock
            .Setup(e => e.Invoke(It.IsAny<int>()))
            .Returns((int value) => value <= maxValid);

        // WHEN ForEach is applied to the collection
        values.ForEach(doAndContinueMock.Object);

        // THEN the delegate was called count times
        doAndContinueMock.Verify(
            e => e.Invoke(It.IsAny<int>()),
            Times.Exactly(maxValid + 1));

        // And for each element in the collection up to maxValid + 1
        foreach (var value in Enumerable.Range(1, maxValid + 1))
        {
            // The delegate was called once per each value
            doAndContinueMock.Verify(
                e => e.Invoke(value),
                Times.Once);
        }

        // And for each element in the collection above maxValid + 1
        foreach (var value in Enumerable
                     .Range(maxValid + 2, count - maxValid - 2))
        {
            // The delegate was not called
            doAndContinueMock.Verify(
                e => e.Invoke(value),
                Times.Never);
        }
    }

    [Fact]
    public void ForEach_All()
    {
        // GIVEN a collection size
        var count = 9;

        // And a collection with count elements
        var values = Enumerable.Range(1, count);

        // And a delegate to receive items
        var doActionMock = new Mock<Action<int>>(MockBehavior.Loose);

        // WHEN ForEach is applied to the collection
        values.ForEach(doActionMock.Object);

        // THEN the delegate was called count times
        doActionMock.Verify(
            e => e.Invoke(It.IsAny<int>()),
            Times.Exactly(count));

        // And for each element in the collection
        foreach (var value in values)
        {
            // The delegate was called once per each value
            doActionMock.Verify(e => e.Invoke(value), Times.Once);
        }
    }

    [Fact]
    public async Task ForEachAsync_While()
    {
        // GIVEN a collection size
        var count = 9;

        // And a maximum value valid for the delegate
        var maxValid = 5;

        // And a collection with count elements
        var values = Enumerable.Range(1, count);

        // And a delegate to receive items
        var doAndContinueMock = new Mock<Func<int, Task<bool>>>(MockBehavior.Strict);
        doAndContinueMock
            .Setup(e => e.Invoke(It.IsAny<int>()))
            .Returns(async (int value) => value <= maxValid);

        // WHEN ForEach is applied to the collection
        await values.ForEachAsync(doAndContinueMock.Object);

        // THEN the delegate was called count times
        doAndContinueMock.Verify(
            e => e.Invoke(It.IsAny<int>()),
            Times.Exactly(maxValid + 1));

        // And for each element in the collection up to maxValid + 1
        foreach (var value in Enumerable.Range(1, maxValid + 1))
        {
            // The delegate was called once per each value
            doAndContinueMock.Verify(
                e => e.Invoke(value),
                Times.Once);
        }

        // And for each element in the collection above maxValid + 1
        foreach (var value in Enumerable
                     .Range(maxValid + 2, count - maxValid - 2))
        {
            // The delegate was not called
            doAndContinueMock.Verify(
                e => e.Invoke(value),
                Times.Never);
        }
    }

    [Fact]
    public async Task ForEachAsync_All()
    {
        // GIVEN a collection size
        var count = 9;

        // And a collection with count elements
        var values = Enumerable.Range(1, count);

        // And a delegate to receive items
        var doActionMock = new Mock<Func<int, Task>>(MockBehavior.Loose);

        // WHEN ForEach is applied to the collection
        await values.ForEachAsync(doActionMock.Object);

        // THEN the delegate was called count times
        doActionMock.Verify(
            e => e.Invoke(It.IsAny<int>()),
            Times.Exactly(count));

        // And for each element in the collection
        foreach (var value in values)
        {
            // The delegate was called once per each value
            doActionMock.Verify(e => e.Invoke(value), Times.Once);
        }
    }

    [Fact]
    public async Task ForEachAsync_Cancellable_While()
    {
        // GIVEN a collection size
        var count = 9;

        // And a maximum value valid for the delegate
        var maxValid = 5;

        // And a collection with count elements
        var values = Enumerable.Range(1, count);

        // And a cancellation token source
        var tokenSource = new CancellationTokenSource();

        // And a delegate to receive items
        var doAndContinueMock = new Mock<Func<int, CancellationToken, Task<bool>>>(MockBehavior.Strict);
        doAndContinueMock
            .Setup(e => e.Invoke(It.IsAny<int>(), tokenSource.Token))
            .Returns(async (int value, CancellationToken token) => value <= maxValid);

        // WHEN ForEach is applied to the collection
        await values.ForEachAsync(doAndContinueMock.Object, tokenSource.Token);

        // THEN the delegate was called count times
        doAndContinueMock.Verify(
            e => e.Invoke(It.IsAny<int>(), tokenSource.Token),
            Times.Exactly(maxValid + 1));

        // And for each element in the collection up to maxValid + 1
        foreach (var value in Enumerable.Range(1, maxValid + 1))
        {
            // The delegate was called once per each value
            doAndContinueMock.Verify(
                e => e.Invoke(value, tokenSource.Token),
                Times.Once);
        }

        // And for each element in the collection above maxValid + 1
        foreach (var value in Enumerable
                     .Range(maxValid + 2, count - maxValid - 2))
        {
            // The delegate was not called
            doAndContinueMock.Verify(
                e => e.Invoke(value, tokenSource.Token),
                Times.Never);
        }
    }

    [Fact]
    public async Task ForEachAsync_Cancellable_All()
    {
        // GIVEN a collection size
        var count = 9;

        // And a collection with count elements
        var values = Enumerable.Range(1, count);

        // And a cancellation token source
        var tokenSource = new CancellationTokenSource();

        // And a delegate to receive items
        var doActionMock = new Mock<Func<int, CancellationToken, Task>>(MockBehavior.Loose);

        // WHEN ForEach is applied to the collection
        await values.ForEachAsync(doActionMock.Object, tokenSource.Token);

        // THEN the delegate was called count times
        doActionMock.Verify(
            e => e.Invoke(It.IsAny<int>(), tokenSource.Token),
            Times.Exactly(count));

        // And for each element in the collection
        foreach (var value in values)
        {
            // The delegate was called once per each value
            doActionMock.Verify(
                e => e.Invoke(value, tokenSource.Token),
                Times.Once);
        }
    }
}
