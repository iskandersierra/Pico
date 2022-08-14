namespace Pico.Tests;

public partial class TaskExtensionsTests
{
    [Fact]
    public async Task AsValueTaskFromResult()
    {
        // Given
        Task task = Task.FromResult(42);

        // When
        var action = async () => await task.AsValueTask();

        // Then
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AsValueTaskFromException()
    {
        // Given
        Task task = Task.FromException(new InvalidOperationException());

        // When
        var action = async () => await task.AsValueTask();

        // Then
        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task AsValueTaskOfTFromResult()
    {
        // Given
        var task = Task.FromResult(42);

        // When
        var result = await task.AsValueTask();

        // Then
        result.Should().Be(42);
    }

    [Fact]
    public async Task AsValueTaskOfTFromException()
    {
        // Given
        var task = Task.FromException<int>(new InvalidOperationException());

        // When
        var action = async () => await task.AsValueTask();

        // Then
        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ToFailedTask()
    {
        // Given
        var task = new InvalidOperationException().ToFailedTask();

        // When
        var action = async () => await task;

        // Then
        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ToFailedTaskOfT()
    {
        // Given
        var task = new InvalidOperationException().ToFailedTask<int>();

        // When
        var action = async () => await task;

        // Then
        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ToFailedValueTask()
    {
        // Given
        var task = new InvalidOperationException().ToFailedValueTask();

        // When
        var action = async () => await task;

        // Then
        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ToFailedValueTaskOfT()
    {
        // Given
        var task = new InvalidOperationException().ToFailedValueTask<int>();

        // When
        var action = async () => await task;

        // Then
        await action.Should().ThrowAsync<InvalidOperationException>();
    }
}
