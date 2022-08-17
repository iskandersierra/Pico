using System.Security.Cryptography;

namespace Pico.Abstractions.IdGenerators;

public class CryptoBinaryIdGenerator :
    IBinaryIdGenerator
{
    public CryptoBinaryIdGenerator(
        int idLength)
    {
        if (idLength <= 0 || idLength > 4096)
            throw new ArgumentOutOfRangeException(
                nameof(idLength), idLength, "Must be between 1 and 4096 (4Kb)");

        IdLength = idLength;
    }

    public int IdLength { get; }

    public void Generate(Span<byte> buffer)
    {
        if (IdLength != buffer.Length)
            throw new ArgumentOutOfRangeException(
                nameof(buffer),
                $"Expected a buffer with length {IdLength}, but had {buffer.Length}");

        RandomNumberGenerator.Fill(buffer);
    }
}
