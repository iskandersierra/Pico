using System.Buffers;

namespace Pico.Abstractions.IdGenerators;

public interface IGenericIdGenerator<T>
{
    int IdLength { get; }

    void Generate(Span<T> buffer);

    public void Generate(T[] buffer)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));

        if (buffer.Length != IdLength)
            throw new ArgumentException(
                $"{nameof(buffer)} must be {IdLength} elements long", nameof(buffer));

        Generate(buffer.AsSpan());
    }

    public void Generate(T[] buffer, int offset)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));

        if (offset < 0 || offset >= buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(offset));

        if (buffer.Length - offset < IdLength)
            throw new ArgumentException(
                $"{nameof(buffer)} must be at least {IdLength} elements long", nameof(buffer));

        Generate(buffer.AsSpan(offset, IdLength));
    }

    public T[] Generate(Func<int, T[]> createArray)
    {
        var buffer = createArray(IdLength);
        Generate(buffer.AsSpan());
        return buffer;
    }

    public T[] Generate(ArrayPool<T> arrayPool) =>
        Generate(arrayPool.Rent);

    public T[] Generate() => Generate(size => new T[size]);
}
