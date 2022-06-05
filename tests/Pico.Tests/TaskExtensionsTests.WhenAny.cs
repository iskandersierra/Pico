namespace Pico.Tests;

public partial class TaskExtensionsTests
{
    [Fact]
    public async Task WhenAnyTyped()
    {
        var count = 5;
        var tasks = Enumerable.Range(0, count)
            .Select(Task.FromResult)
            .ToArray();

        var taskOfTask = tasks.WhenAny();

        var task = await taskOfTask;

        tasks.Should().Contain(task);

        var result = await task;

        Enumerable.Range(0, count).Should().Contain(result);
    }

    [Fact]
    public async Task WhenAnyTask()
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

        var tasks = mocks
            .Select(mock => mock.Object())
            .ToArray();

        var taskOfTask = tasks.WhenAny();

        var task = await taskOfTask;

        tasks.Should().Contain(task);

        foreach (var mock in mocks)
        {
            mock.Verify(e => e.Invoke(), Times.Once);
        }
    }

    [Fact]
    public async Task WhenAnyTwoTyped()
    {
        var tasks = Enumerable.Range(0, 2)
            .Select(Task.FromResult)
            .ToArray();

        var taskOfTask = tasks[0].WhenAny(tasks[1]);

        var task = await taskOfTask;

        tasks.Should().Contain(task);

        var result = await task;

        Enumerable.Range(0, 2).Should().Contain(result);
    }

    [Fact]
    public async Task WhenAnyTwoTask()
    {
        var count = 2;

        var mocks = Enumerable.Range(0, count)
            .Select(i =>
            {
                var mock = new Mock<Func<Task>>(MockBehavior.Strict);
                mock.Setup(e => e.Invoke())
                    .Returns(async () => { await Task.Yield(); });
                return mock;
            })
            .ToArray();

        var tasks = mocks
            .Select(mock => mock.Object())
            .ToArray();

        var taskOfTask = tasks[0].WhenAny(tasks[1]);

        var task = await taskOfTask;

        tasks.Should().Contain(task);

        foreach (var mock in mocks)
        {
            mock.Verify(e => e.Invoke(), Times.Once);
        }
    }

    [Fact]
    public async Task WhenAnyParamsTyped()
    {
        var count = 5;
        var tasks = Enumerable.Range(0, count)
            .Select(Task.FromResult)
            .ToArray();

        var taskOfTask = tasks[0].WhenAny(tasks[1], tasks[2], tasks[3], tasks[4]);

        var task = await taskOfTask;

        tasks.Should().Contain(task);

        var result = await task;

        Enumerable.Range(0, count).Should().Contain(result);
    }

    [Fact]
    public async Task WhenAnyParamsTask()
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

        var tasks = mocks
            .Select(mock => mock.Object())
            .ToArray();

        var taskOfTask = tasks[0].WhenAny(tasks[1], tasks[2], tasks[3], tasks[4]);

        var task = await taskOfTask;

        tasks.Should().Contain(task);

        foreach (var mock in mocks)
        {
            mock.Verify(e => e.Invoke(), Times.Once);
        }
    }
}
