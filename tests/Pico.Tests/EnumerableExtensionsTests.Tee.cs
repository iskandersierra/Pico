using FsCheck;

namespace Pico.Tests;

public partial class EnumerableExtensionsTests
{
    [Property]
    public void Tee(NonEmptyArray<int> values)
    {
        // GIVEN a collection
        var action = new Mock<Action<int>>(MockBehavior.Strict);
        action
            .Setup(e => e.Invoke(It.IsAny<int>()))
            .Callback(() => { /* Ignore */ });

        // WHEN a Tee is applied to the collection
        var target = values.Get.Tee(action.Object);

        // THEN target is not null
        target.Should().NotBeNull();

        // AND action should not be called before consuming
        action.Verify(
            e => e.Invoke(It.IsAny<int>()),
            Times.Never);

        // AND target should be equivalent to the original collection
        target.Should().BeEquivalentTo(
            values.Get,
            options => options.WithStrictOrdering());

        // AND action should be called after consuming
        action.Verify(
            e => e.Invoke(It.IsAny<int>()),
            Times.Exactly(values.Get.Length));
    }

    [Property]
    public void TeeAsync(NonEmptyArray<int> values)
    {
        // GIVEN a collection
        var action = new Mock<Func<int, Task>>(MockBehavior.Strict);
        action
            .Setup(e => e.Invoke(It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // WHEN a TeeAsync is applied to the collection
        var target = values.Get.TeeAsync(action.Object);

        // THEN target is not null
        target.Should().NotBeNull();

        // AND action should not be called before consuming
        action.Verify(
            e => e.Invoke(It.IsAny<int>()),
            Times.Never);

        // AND target should be equivalent to the original collection
        target.ToArrayAsync()
            .ConfigureAwait(false).GetAwaiter().GetResult()
            .Should().BeEquivalentTo(
                values.Get,
                options => options.WithStrictOrdering());

        // AND action should be called after consuming
        action.Verify(
            e => e.Invoke(It.IsAny<int>()),
            Times.Exactly(values.Get.Length));
    }

    [Property]
    public void TeeAsyncWithCancellationToken(NonEmptyArray<int> values)
    {
        // GIVEN a collection
        var action = new Mock<Func<int, CancellationToken, Task>>(MockBehavior.Strict);
        action
            .Setup(e => e.Invoke(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // AND a cancellation token
        var cancellationToken = new CancellationTokenSource().Token;

        // WHEN a TeeAsync is applied to the collection
        var target = values.Get.TeeAsync(action.Object, cancellationToken);

        // THEN target is not null
        target.Should().NotBeNull();

        // AND action should not be called before consuming
        action.Verify(
            e => e.Invoke(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        // AND target should be equivalent to the original collection
        target.ToArrayAsync(cancellationToken)
            .ConfigureAwait(false).GetAwaiter().GetResult()
            .Should().BeEquivalentTo(
                values.Get,
                options => options.WithStrictOrdering());

        // AND action should be called after consuming
        action.Verify(
            e => e.Invoke(It.IsAny<int>(), cancellationToken),
            Times.Exactly(values.Get.Length));
    }
}
