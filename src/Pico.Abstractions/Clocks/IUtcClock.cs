namespace Pico.Abstractions.Clocks;

public interface IUtcClock
{
    DateTime UtcNow { get; }
}

public class SystemUtcClock : IUtcClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}

public class FixedUtcClock : IUtcClock
{
    public FixedUtcClock(DateTime now)
    {
        UtcNow = now;
    }

    public DateTime UtcNow { get; }
}
