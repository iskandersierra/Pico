namespace Pico.Tests;

public partial class TaskExtensionsTests
{
    [Fact]
    public async Task WhenAllTyped()
    {
        var count = 5;
        var tasks = Enumerable.Range(0, count).Select(Task.FromResult);

        var task = tasks.WhenAll();

        var result = await task;

        result.Should().BeEquivalentTo(Enumerable.Range(0, count));
    }

    [Fact]
    public async Task WhenAllTask()
    {
        var count = 5;

        var mocks = Enumerable.Range(0, count)
            .Select(i =>
            {
                var mock = new Mock<Func<Task>>(MockBehavior.Strict);
                mock.Setup(e => e.Invoke())
                    .Returns(async () => { await Task.Yield(); });
                return mock;
            })
            .ToArray();

        var tasks = Enumerable.Range(0, count)
            .Select(index => mocks[index].Object());

        var task = tasks.WhenAll();

        await task;

        foreach (var mock in mocks)
        {
            mock.Verify(e => e.Invoke(), Times.Once);
        }
    }
}
