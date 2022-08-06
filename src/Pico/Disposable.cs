namespace Pico;

public static class Disposable
{
    public static IAsyncDisposable AsAsyncDisposable(this IDisposable disposable) =>
        FromValueTask(async () =>
        {
            disposable.Dispose();
            await ValueTask.CompletedTask;
        });

    public static IDisposable From(Action action) =>
        new ActionDisposable(action);

    public static IAsyncDisposable FromTask(Func<Task> action) =>
        new FuncTaskDisposable(action);

    public static IAsyncDisposable FromValueTask(Func<ValueTask> action) =>
        new FuncValueTaskDisposable(action);

    public static ICompositeDisposable Composite(IEnumerable<IDisposable> disposables)
    {
        ICompositeDisposable composite = new CompositeDisposable();
        composite.AddRange(disposables);
        return composite;
    }

    public static ICompositeDisposable Composite(params IDisposable[] disposables) =>
        Composite((IEnumerable<IDisposable>)disposables);

    public static ICompositeAsyncDisposable CompositeAsync(IEnumerable<IAsyncDisposable> disposables)
    {
        ICompositeAsyncDisposable composite = new CompositeAsyncDisposable();
        composite.AddRange(disposables);
        return composite;
    }

    public static ICompositeAsyncDisposable CompositeAsync(params IAsyncDisposable[] disposables) =>
        CompositeAsync((IEnumerable<IAsyncDisposable>)disposables);

    class ActionDisposable : IDisposable
    {
        private Action? action;

        public ActionDisposable(Action action) =>
            this.action = action ?? throw new ArgumentNullException(nameof(action));

        public void Dispose()
        {
            if (action is not { } f) return;
            action = null;
            f();
        }
    }

    class FuncTaskDisposable : IAsyncDisposable
    {
        private Func<Task>? action;

        public FuncTaskDisposable(Func<Task> action) =>
            this.action = action ?? throw new ArgumentNullException(nameof(action));

        public async ValueTask DisposeAsync()
        {
            if (action is not { } f) return;
            action = null;
            await f();
        }
    }

    class FuncValueTaskDisposable : IAsyncDisposable
    {
        private Func<ValueTask>? action;

        public FuncValueTaskDisposable(Func<ValueTask> action) =>
            this.action = action ?? throw new ArgumentNullException(nameof(action));

        public ValueTask DisposeAsync()
        {
            if (action is not { } f) return ValueTask.CompletedTask;
            action = null;
            return f();

        }
    }

    class CompositeDisposable : ICompositeDisposable
    {
        private List<IDisposable> disposables = new();

        public int Count => disposables.Count;

        public ICompositeDisposable Add(IDisposable disposable)
        {
            disposables.Add(disposable ?? throw new ArgumentNullException(nameof(disposable)));
            return this;
        }

        public void Dispose()
        {
            if (disposables.Count <= 0) return;
            var buffer = disposables.ToArray();
            disposables.Clear();
            buffer.ForEach(d => d.Dispose());
        }
    }

    class CompositeAsyncDisposable : ICompositeAsyncDisposable
    {
        private List<IAsyncDisposable> disposables = new();

        public int Count => disposables.Count;

        public ICompositeAsyncDisposable Add(IAsyncDisposable disposable)
        {
            disposables.Add(disposable ?? throw new ArgumentNullException(nameof(disposable)));
            return this;
        }

        public async ValueTask DisposeAsync()
        {
            if (disposables.Count <= 0) return;
            var buffer = disposables.ToArray();
            disposables.Clear();
            await buffer.ForEachAsync(d => d.DisposeAsync());
        }
    }
}

public interface ICompositeDisposable : IDisposable
{
    int Count { get; }

    ICompositeDisposable Add(IDisposable disposable);

    public ICompositeDisposable Add(Action action) =>
        Add(Disposable.From(action));

    public ICompositeDisposable AddRange(IEnumerable<IDisposable> disposables)
    {
        disposables.ForEach(d => Add(d));
        return this;
    }

    public ICompositeDisposable AddRange(params IDisposable[] disposables) =>
        AddRange((IEnumerable<IDisposable>)disposables);
}

public interface ICompositeAsyncDisposable : IAsyncDisposable
{
    int Count { get; }

    ICompositeAsyncDisposable Add(IAsyncDisposable disposable);

    public ICompositeAsyncDisposable Add(Func<Task> action) =>
        Add(Disposable.FromTask(action));

    public ICompositeAsyncDisposable Add(Func<ValueTask> action) =>
        Add(Disposable.FromValueTask(action));

    public ICompositeAsyncDisposable AddRange(IEnumerable<IAsyncDisposable> disposables)
    {
        disposables.ForEach(d => Add(d));
        return this;
    }

    public ICompositeAsyncDisposable AddRange(params IAsyncDisposable[] disposables) =>
        AddRange((IEnumerable<IAsyncDisposable>)disposables);
}
