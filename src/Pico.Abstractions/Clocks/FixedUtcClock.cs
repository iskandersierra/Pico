namespace Pico.Abstractions.Clocks;

public class FixedUtcClock : IUtcClock
{
    public FixedUtcClock(DateTime now)
    {
        UtcNow = now;
    }

    public DateTime UtcNow { get; }
}
