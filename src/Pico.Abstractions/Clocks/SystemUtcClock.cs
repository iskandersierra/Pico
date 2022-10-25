namespace Pico.Abstractions.Clocks;

public class SystemUtcClock : IUtcClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
