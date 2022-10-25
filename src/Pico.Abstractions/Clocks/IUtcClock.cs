namespace Pico.Abstractions.Clocks;

public interface IUtcClock
{
    DateTime UtcNow { get; }
}
