namespace Pico;

/// <summary>
/// Extension methods for <see cref="Task"/> and related.
/// </summary>
public static partial class TaskExtensions
{
    public static ValueTask AsValueTask(this Task task)
    {
        return new ValueTask(task);
    }

    public static ValueTask<T> AsValueTask<T>(this Task<T> task)
    {
        return new ValueTask<T>(task);
    }

    public static Task ToFailedTask(this Exception exception)
    {
        return Task.FromException(exception);
    }

    public static Task<T> ToFailedTask<T>(this Exception exception)
    {
        return Task.FromException<T>(exception);
    }

    public static ValueTask ToFailedValueTask(this Exception exception)
    {
        return exception.ToFailedTask().AsValueTask();
    }

    public static ValueTask<T> ToFailedValueTask<T>(this Exception exception)
    {
        return exception.ToFailedTask<T>().AsValueTask();
    }
}
