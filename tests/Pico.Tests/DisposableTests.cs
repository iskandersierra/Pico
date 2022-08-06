namespace Pico.Tests;

public class DisposableTests
{
    [Fact]
    public void AsAsyncDisposableCreate()
    {
        // GIVEN a disposable
        var disposable = new Mock<IDisposable>(MockBehavior.Strict);

        // WHEN an async disposable is created
        disposable.Object.AsAsyncDisposable();

        // THEN the disposable is not disposed
        disposable.Verify(d => d.Dispose(), Times.Never);
    }

    [Fact]
    public async Task AsAsyncDisposableDispose()
    {
        // GIVEN a disposable
        var disposable = new Mock<IDisposable>(MockBehavior.Strict);
        disposable.Setup(d => d.Dispose()).Callback(() => { /* Ignore */ });

        // AND an async disposable is created from the disposable
        var asyncDisposable = disposable.Object.AsAsyncDisposable();

        // WHEN the disposable is disposed
        await asyncDisposable.DisposeAsync();

        // THEN the disposable is disposed
        disposable.Verify(d => d.Dispose(), Times.Once);
    }


    [Fact]
    public void FromCreate()
    {
        // GIVEN an action
        var action = new Mock<Action>(MockBehavior.Strict);

        // WHEN a disposable is created from the action
        Disposable.From(action.Object);

        // THEN the action is not called
        action.Verify(a => a(), Times.Never);
    }

    [Fact]
    public void FromDispose()
    {
        // GIVEN an action
        var action = new Mock<Action>(MockBehavior.Strict);
        action.Setup(a => a()).Callback(() => { /* Ignore */ });

        // AND a disposable is created from the action
        var disposable = Disposable.From(action.Object);

        // WHEN the disposable is disposed once
        disposable.Dispose();

        // THEN the action is called once
        action.Verify(a => a(), Times.Once);
    }

    [Fact]
    public void FromDisposeMultiple()
    {
        // GIVEN an action
        var action = new Mock<Action>(MockBehavior.Strict);
        action.Setup(a => a()).Callback(() => {/* Ignore */ });

        // AND a disposable is created from the action
        var disposable = Disposable.From(action.Object);

        // WHEN the disposable is disposed multiple times
        disposable.Dispose();
        disposable.Dispose();
        disposable.Dispose();

        // THEN the action is called once
        action.Verify(a => a(), Times.Once);
    }


    [Fact]
    public void FromTaskCreate()
    {
        // GIVEN an action
        var action = new Mock<Func<Task>>(MockBehavior.Strict);

        // WHEN a disposable is created from the action
        Disposable.FromTask(action.Object);

        // THEN the action is not called
        action.Verify(a => a(), Times.Never);
    }

    [Fact]
    public async Task FromTaskDispose()
    {
        // GIVEN an action
        var action = new Mock<Func<Task>>(MockBehavior.Strict);
        action.Setup(a => a()).Returns(Task.CompletedTask);

        // AND a disposable is created from the action
        var disposable = Disposable.FromTask(action.Object);

        // WHEN the disposable is disposed once
        await disposable.DisposeAsync();

        // THEN the action is called once
        action.Verify(a => a(), Times.Once);
    }

    [Fact]
    public async Task FromTaskDisposeMultiple()
    {
        // GIVEN an action
        var action = new Mock<Func<Task>>(MockBehavior.Strict);
        action.Setup(a => a()).Returns(Task.CompletedTask);

        // AND a disposable is created from the action
        var disposable = Disposable.FromTask(action.Object);

        // WHEN the disposable is disposed multiple times
        await disposable.DisposeAsync();
        await disposable.DisposeAsync();
        await disposable.DisposeAsync();

        // THEN the action is called once
        action.Verify(a => a(), Times.Once);
    }


    [Fact]
    public void FromValueTaskCreate()
    {
        // GIVEN an action
        var action = new Mock<Func<ValueTask>>(MockBehavior.Strict);

        // WHEN a disposable is created from the action
        Disposable.FromValueTask(action.Object);

        // THEN the action is not called
        action.Verify(a => a(), Times.Never);
    }

    [Fact]
    public async ValueTask FromValueTaskDispose()
    {
        // GIVEN an action
        var action = new Mock<Func<ValueTask>>(MockBehavior.Strict);
        action.Setup(a => a()).Returns(ValueTask.CompletedTask);

        // AND a disposable is created from the action
        var disposable = Disposable.FromValueTask(action.Object);

        // WHEN the disposable is disposed once
        await disposable.DisposeAsync();

        // THEN the action is called once
        action.Verify(a => a(), Times.Once);
    }

    [Fact]
    public async ValueTask FromValueTaskDisposeMultiple()
    {
        // GIVEN an action
        var action = new Mock<Func<ValueTask>>(MockBehavior.Strict);
        action.Setup(a => a()).Returns(ValueTask.CompletedTask);

        // AND a disposable is created from the action
        var disposable = Disposable.FromValueTask(action.Object);

        // WHEN the disposable is disposed multiple times
        await disposable.DisposeAsync();
        await disposable.DisposeAsync();
        await disposable.DisposeAsync();

        // THEN the action is called once
        action.Verify(a => a(), Times.Once);
    }


    [Fact]
    public void CompositeCreate()
    {
        // GIVEN three disposables
        var disposable1 = new Mock<IDisposable>(MockBehavior.Strict);
        var disposable2 = new Mock<IDisposable>(MockBehavior.Strict);
        var disposable3 = new Mock<IDisposable>(MockBehavior.Strict);

        // WHEN a composite disposable is created
        var composite = Disposable.Composite(
            disposable1.Object, disposable2.Object, disposable3.Object);

        // THEN the disposables are not disposed
        disposable1.Verify(d => d.Dispose(), Times.Never);
        disposable2.Verify(d => d.Dispose(), Times.Never);
        disposable3.Verify(d => d.Dispose(), Times.Never);

        // AND disposable count is 3
        composite.Count.Should().Be(3);
    }

    [Fact]
    public void CompositeDispose()
    {
        // GIVEN three disposables
        var disposable1 = new Mock<IDisposable>(MockBehavior.Strict);
        disposable1.Setup(d => d.Dispose()).Callback(() => { /* Ignore */ });
        var disposable2 = new Mock<IDisposable>(MockBehavior.Strict);
        disposable2.Setup(d => d.Dispose()).Callback(() => { /* Ignore */ });
        var disposable3 = new Mock<IDisposable>(MockBehavior.Strict);
        disposable3.Setup(d => d.Dispose()).Callback(() => { /* Ignore */ });

        // AND a composite disposable is created
        var composite = Disposable.Composite(
            disposable1.Object, disposable2.Object, disposable3.Object);

        // WHEN the composite is disposed
        composite.Dispose();

        // THEN the disposables are disposed
        disposable1.Verify(d => d.Dispose(), Times.Once);
        disposable2.Verify(d => d.Dispose(), Times.Once);
        disposable3.Verify(d => d.Dispose(), Times.Once);

        // AND the composite is empty
        composite.Count.Should().Be(0);
    }

    [Fact]
    public void CompositeDisposeMultiple()
    {
        // GIVEN three disposables
        var disposable1 = new Mock<IDisposable>(MockBehavior.Strict);
        disposable1.Setup(d => d.Dispose()).Callback(() => { /* Ignore */ });
        var disposable2 = new Mock<IDisposable>(MockBehavior.Strict);
        disposable2.Setup(d => d.Dispose()).Callback(() => { /* Ignore */ });
        var disposable3 = new Mock<IDisposable>(MockBehavior.Strict);
        disposable3.Setup(d => d.Dispose()).Callback(() => { /* Ignore */ });

        // AND a composite disposable is created
        var composite = Disposable.Composite(
            disposable1.Object, disposable2.Object, disposable3.Object);

        // WHEN the composite is disposed multiple times
        composite.Dispose();
        composite.Dispose();
        composite.Dispose();

        // THEN the disposables are disposed
        disposable1.Verify(d => d.Dispose(), Times.Once);
        disposable2.Verify(d => d.Dispose(), Times.Once);
        disposable3.Verify(d => d.Dispose(), Times.Once);

        // AND the composite is empty
        composite.Count.Should().Be(0);
    }

    [Fact]
    public void CompositeAddOneMoreBeforeDispose()
    {
        // GIVEN three disposables
        var disposable1 = new Mock<IDisposable>(MockBehavior.Strict);
        disposable1.Setup(d => d.Dispose()).Callback(() => { /* Ignore */ });
        var disposable2 = new Mock<IDisposable>(MockBehavior.Strict);
        disposable2.Setup(d => d.Dispose()).Callback(() => { /* Ignore */ });
        var action3 = new Mock<Action>(MockBehavior.Strict);
        action3.Setup(d => d.Invoke()).Callback(() => { /* Ignore */ });

        // AND a composite disposable is created
        var composite = Disposable.Composite(
            disposable1.Object, disposable2.Object);

        // AND another disposable is added
        composite.Add(action3.Object);

        // THEN the composite has 3 disposables
        composite.Count.Should().Be(3);

        // WHEN the composite is disposed
        composite.Dispose();

        // THEN the disposables are disposed
        disposable1.Verify(d => d.Dispose(), Times.Once);
        disposable2.Verify(d => d.Dispose(), Times.Once);
        action3.Verify(d => d.Invoke(), Times.Once);

        // AND the composite is empty
        composite.Count.Should().Be(0);
    }

    [Fact]
    public void CompositeAddOneMoreAfterDispose()
    {
        // GIVEN three disposables
        var disposable1 = new Mock<IDisposable>(MockBehavior.Strict);
        disposable1.Setup(d => d.Dispose()).Callback(() => { /* Ignore */ });
        var disposable2 = new Mock<IDisposable>(MockBehavior.Strict);
        disposable2.Setup(d => d.Dispose()).Callback(() => { /* Ignore */ });
        var action3 = new Mock<Action>(MockBehavior.Strict);
        action3.Setup(d => d.Invoke()).Callback(() => { /* Ignore */ });

        // AND a composite disposable is created
        var composite = Disposable.Composite(
            disposable1.Object, disposable2.Object);

        // THEN the composite has 2 disposables
        composite.Count.Should().Be(2);

        // WHEN the composite is disposed
        composite.Dispose();

        // THEN only the first two disposables are disposed
        disposable1.Verify(d => d.Dispose(), Times.Once);
        disposable2.Verify(d => d.Dispose(), Times.Once);
        action3.Verify(d => d.Invoke(), Times.Never);

        // AND the composite is empty
        composite.Count.Should().Be(0);

        // WHEN another disposable is added
        composite.Add(action3.Object);

        // THEN the composite has one disposables
        composite.Count.Should().Be(1);

        // AND the composite has the new disposable
        composite.Count.Should().Be(1);

        // AND the new disposable is still not disposed
        action3.Verify(d => d.Invoke(), Times.Never);

        // WHEN the composite is disposed again
        composite.Dispose();

        // THEN only the new disposable is disposed
        action3.Verify(d => d.Invoke(), Times.Once);
        disposable1.Verify(d => d.Dispose(), Times.Once);
        disposable2.Verify(d => d.Dispose(), Times.Once);

        // AND the composite is empty
        composite.Count.Should().Be(0);
    }

    [Fact]
    public void CompositeAddMultiple()
    {
        // GIVEN three disposables
        var disposable1 = new Mock<IDisposable>(MockBehavior.Strict);
        disposable1.Setup(d => d.Dispose()).Callback(() => { /* Ignore */ });
        var disposable2 = new Mock<IDisposable>(MockBehavior.Strict);
        disposable2.Setup(d => d.Dispose()).Callback(() => { /* Ignore */ });
        var disposable3 = new Mock<IDisposable>(MockBehavior.Strict);
        disposable3.Setup(d => d.Dispose()).Callback(() => { /* Ignore */ });
        var disposable4 = new Mock<IDisposable>(MockBehavior.Strict);
        disposable4.Setup(d => d.Dispose()).Callback(() => { /* Ignore */ });

        // AND a composite disposable is created
        var composite = Disposable.Composite(
            disposable1.Object, disposable2.Object);

        // AND another disposables are added
        composite.AddRange(disposable3.Object, disposable4.Object);

        // THEN the composite has 4 disposables
        composite.Count.Should().Be(4);

        // WHEN the composite is disposed
        composite.Dispose();

        // THEN the disposables are disposed
        disposable1.Verify(d => d.Dispose(), Times.Once);
        disposable2.Verify(d => d.Dispose(), Times.Once);
        disposable3.Verify(d => d.Dispose(), Times.Once);
        disposable4.Verify(d => d.Dispose(), Times.Once);

        // AND the composite is empty
        composite.Count.Should().Be(0);
    }


    [Fact]
    public void CompositeAsyncCreate()
    {
        // GIVEN three disposables
        var disposable1 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        var disposable2 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        var disposable3 = new Mock<IAsyncDisposable>(MockBehavior.Strict);

        // WHEN a composite disposable is created
        var composite = Disposable.CompositeAsync(
            disposable1.Object, disposable2.Object, disposable3.Object);

        // THEN the disposables are not disposed
        disposable1.Verify(d => d.DisposeAsync(), Times.Never);
        disposable2.Verify(d => d.DisposeAsync(), Times.Never);
        disposable3.Verify(d => d.DisposeAsync(), Times.Never);

        // AND disposable count is 3
        composite.Count.Should().Be(3);
    }

    [Fact]
    public async Task CompositeAsyncDispose()
    {
        // GIVEN three disposables
        var disposable1 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        disposable1.Setup(d => d.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var disposable2 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        disposable2.Setup(d => d.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var disposable3 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        disposable3.Setup(d => d.DisposeAsync()).Returns(ValueTask.CompletedTask);

        // AND a composite disposable is created
        var composite = Disposable.CompositeAsync(
            disposable1.Object, disposable2.Object, disposable3.Object);

        // WHEN the composite is disposed
        await composite.DisposeAsync();

        // THEN the disposables are disposed
        disposable1.Verify(d => d.DisposeAsync(), Times.Once);
        disposable2.Verify(d => d.DisposeAsync(), Times.Once);
        disposable3.Verify(d => d.DisposeAsync(), Times.Once);

        // AND the composite is empty
        composite.Count.Should().Be(0);
    }

    [Fact]
    public async Task CompositeAsyncDisposeMultiple()
    {
        // GIVEN three disposables
        var disposable1 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        disposable1.Setup(d => d.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var disposable2 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        disposable2.Setup(d => d.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var disposable3 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        disposable3.Setup(d => d.DisposeAsync()).Returns(ValueTask.CompletedTask);

        // AND a composite disposable is created
        var composite = Disposable.CompositeAsync(
            disposable1.Object, disposable2.Object, disposable3.Object);

        // WHEN the composite is disposed multiple times
        await composite.DisposeAsync();
        await composite.DisposeAsync();
        await composite.DisposeAsync();

        // THEN the disposables are disposed
        disposable1.Verify(d => d.DisposeAsync(), Times.Once);
        disposable2.Verify(d => d.DisposeAsync(), Times.Once);
        disposable3.Verify(d => d.DisposeAsync(), Times.Once);

        // AND the composite is empty
        composite.Count.Should().Be(0);
    }

    [Fact]
    public async Task CompositeAsyncAddOneMoreBeforeDispose()
    {
        // GIVEN three disposables
        var disposable1 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        disposable1.Setup(d => d.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var disposable2 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        disposable2.Setup(d => d.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var action3 = new Mock<Func<Task>>(MockBehavior.Strict);
        action3.Setup(d => d.Invoke()).Returns(Task.CompletedTask);
        var action4 = new Mock<Func<ValueTask>>(MockBehavior.Strict);
        action4.Setup(d => d.Invoke()).Returns(ValueTask.CompletedTask);

        // AND a composite disposable is created
        var composite = Disposable.CompositeAsync(
            disposable1.Object, disposable2.Object);

        // AND another disposable is added
        composite.Add(action3.Object);
        composite.Add(action4.Object);

        // THEN the composite has 4 disposables
        composite.Count.Should().Be(4);

        // WHEN the composite is disposed
        await composite.DisposeAsync();

        // THEN the disposables are disposed
        disposable1.Verify(d => d.DisposeAsync(), Times.Once);
        disposable2.Verify(d => d.DisposeAsync(), Times.Once);
        action3.Verify(d => d.Invoke(), Times.Once);
        action4.Verify(d => d.Invoke(), Times.Once);

        // AND the composite is empty
        composite.Count.Should().Be(0);
    }

    [Fact]
    public async Task CompositeAsyncAddOneMoreAfterDispose()
    {
        // GIVEN three disposables
        var disposable1 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        disposable1.Setup(d => d.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var disposable2 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        disposable2.Setup(d => d.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var disposable3 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        disposable3.Setup(d => d.DisposeAsync()).Returns(ValueTask.CompletedTask);

        // AND a composite disposable is created
        var composite = Disposable.CompositeAsync(
            disposable1.Object, disposable2.Object);

        // THEN the composite has 2 disposables
        composite.Count.Should().Be(2);

        // WHEN the composite is disposed
        await composite.DisposeAsync();

        // THEN only the first two disposables are disposed
        disposable1.Verify(d => d.DisposeAsync(), Times.Once);
        disposable2.Verify(d => d.DisposeAsync(), Times.Once);
        disposable3.Verify(d => d.DisposeAsync(), Times.Never);

        // AND the composite is empty
        composite.Count.Should().Be(0);

        // WHEN another disposable is added
        composite.Add(disposable3.Object);

        // THEN the composite has one disposables
        composite.Count.Should().Be(1);

        // AND the composite has the new disposable
        composite.Count.Should().Be(1);

        // AND the new disposable is still not disposed
        disposable3.Verify(d => d.DisposeAsync(), Times.Never);

        // WHEN the composite is disposed again
        await composite.DisposeAsync();

        // THEN only the new disposable is disposed
        disposable3.Verify(d => d.DisposeAsync(), Times.Once);
        disposable1.Verify(d => d.DisposeAsync(), Times.Once);
        disposable2.Verify(d => d.DisposeAsync(), Times.Once);

        // AND the composite is empty
        composite.Count.Should().Be(0);
    }

    [Fact]
    public async Task CompositeAsyncAddMultiple()
    {
        // GIVEN three disposables
        var disposable1 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        disposable1.Setup(d => d.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var disposable2 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        disposable2.Setup(d => d.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var disposable3 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        disposable3.Setup(d => d.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var disposable4 = new Mock<IAsyncDisposable>(MockBehavior.Strict);
        disposable4.Setup(d => d.DisposeAsync()).Returns(ValueTask.CompletedTask);

        // AND a composite disposable is created
        var composite = Disposable.CompositeAsync(
            disposable1.Object, disposable2.Object);

        // AND another disposables are added
        composite.AddRange(disposable3.Object, disposable4.Object);

        // THEN the composite has 4 disposables
        composite.Count.Should().Be(4);

        // WHEN the composite is disposed
        await composite.DisposeAsync();

        // THEN the disposables are disposed
        disposable1.Verify(d => d.DisposeAsync(), Times.Once);
        disposable2.Verify(d => d.DisposeAsync(), Times.Once);
        disposable3.Verify(d => d.DisposeAsync(), Times.Once);
        disposable4.Verify(d => d.DisposeAsync(), Times.Once);

        // AND the composite is empty
        composite.Count.Should().Be(0);
    }
}
