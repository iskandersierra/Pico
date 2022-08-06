namespace Pico.Abstractions.IdGenerators;

public interface IIdGenerator
{
    int IdLength { get; }

    string Generate();
}

public static class IdGenerator
{
    public static IIdGenerator FromBinary(
        IBinaryIdGenerator binaryGenerator)
    {

    }
}
