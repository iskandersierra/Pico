namespace Pico.Abstractions.IdGenerators;

public interface IIdGenerator
{
    int IdLength { get; }

    string Generate();
}
