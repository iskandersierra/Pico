namespace Pico;

partial class TaskExtensions
{
    /// <summary>
    /// Creates a task that returns the first task out of a list of provided
    /// tasks that first completes.
    /// </summary>
    /// <typeparam name="T">The type of values inside the provided tasks</typeparam>
    /// <param name="tasks">The collection of provided tasks</param>
    /// <returns>A task containing the task that completed first</returns>
    public static Task<Task<T>> WhenAny<T>(
        this IEnumerable<Task<T>> tasks) =>
        Task.WhenAny(tasks);

    /// <summary>
    /// Creates a task that returns the first task out of a list of provided
    /// tasks that first completes.
    /// </summary>
    /// <param name="tasks">The collection of provided tasks</param>
    /// <returns>A task containing the task that completed first</returns>
    public static Task<Task> WhenAny(
        this IEnumerable<Task> tasks) =>
        Task.WhenAny(tasks);

    /// <summary>
    /// Creates a task that returns the first task out of two tasks that first
    /// completes.
    /// </summary>
    /// <typeparam name="T">The type of values inside the provided tasks</typeparam>
    /// <param name="task">The first task to wait for</param>
    /// <param name="secondTask">The second task to wait for</param>
    /// <returns>A task containing the task that completed first</returns>
    public static Task<Task<T>> WhenAny<T>(
        this Task<T> task, Task<T> secondTask) =>
        Task.WhenAny(task, secondTask);

    /// <summary>
    /// Creates a task that returns the first task out of two tasks that first
    /// completes.
    /// </summary>
    /// <param name="task">The first task to wait for</param>
    /// <param name="secondTask">The second task to wait for</param>
    /// <returns>A task containing the task that completed first</returns>
    public static Task<Task> WhenAny(
        this Task task, Task secondTask) =>
        Task.WhenAny(task, secondTask);

    /// <summary>
    /// Creates a task that returns the first task out of a collection of tasks
    /// that first completes.
    /// </summary>
    /// <typeparam name="T">The type of values inside the provided tasks</typeparam>
    /// <param name="task">The first task to wait for</param>
    /// <param name="rest">The rest of tasks to wait for</param>
    /// <returns>A task containing the task that completed first</returns>
    public static Task<Task<T>> WhenAny<T>(
        this Task<T> task, params Task<T>[] rest) =>
        Task.WhenAny(task.AsSingleton().Concat(rest));

    /// <summary>
    /// Creates a task that returns the first task out of a collection of tasks
    /// that first completes.
    /// </summary>
    /// <param name="task">The first task to wait for</param>
    /// <param name="rest">The rest of tasks to wait for</param>
    /// <returns>A task containing the task that completed first</returns>
    public static Task<Task> WhenAny(
        this Task task, params Task[] rest) =>
        Task.WhenAny(task.AsSingleton().Concat(rest));
}
