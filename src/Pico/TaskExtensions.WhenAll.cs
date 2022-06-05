namespace Pico;

partial class TaskExtensions
{
    /// <summary>
    /// Creates a Task that completes when all of the provided tasks have completed.
    /// It's returned value is the array of returned values from the provided tasks.
    /// </summary>
    /// <typeparam name="T">The type of task</typeparam>
    /// <param name="tasks">The list of provided tasks to wait for</param>
    /// <returns>An array of values out of all provided tasks</returns>
    public static Task<T[]> WhenAll<T>(
        this IEnumerable<Task<T>> tasks) =>
        Task.WhenAll(tasks);

    /// <summary>
    /// Creates a Task that completes when all of the provided tasks have completed.
    /// </summary>
    /// <param name="tasks">The list of provided tasks to wait for</param>
    /// <returns>A task that completes when all of the provided tasks have completed.</returns>
    public static Task WhenAll(
        this IEnumerable<Task> tasks) =>
        Task.WhenAll(tasks);
}
