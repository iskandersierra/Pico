namespace Pico.Abstractions.IdGenerators;

public class CompositeGenericIdGenerator<T> :
    IGenericIdGenerator<T>
{
    private readonly (int length, IGenericIdGenerator<T> generator)[] generators;

    public CompositeGenericIdGenerator(
        IEnumerable<(int length, IGenericIdGenerator<T> generator)> generators)
    {
        if (generators == null)
            throw new ArgumentNullException(nameof(generators));

        this.generators = generators.ToArray();

        if (this.generators.Length == 0)
            throw new ArgumentException(
                "At least one generator is required", nameof(generators));

        if (this.generators.Any(g => g.length == 0))
            throw new ArgumentException(
                "Generator length must be greater than zero", nameof(generators));

        this.IdLength = this.generators.Sum(g => g.length);
    }

    public CompositeGenericIdGenerator(
        params (int length, IGenericIdGenerator<T> generator)[] generators) :
        this((IEnumerable<(int length, IGenericIdGenerator<T> generator)>)generators)
    {
    }

    public int IdLength { get; }

    public void Generate(Span<T> buffer)
    {
        if (buffer.Length != IdLength)
            throw new ArgumentException(
                "Buffer must be of length " + IdLength, nameof(buffer));

        var offset = 0;

        foreach (var (length, generator) in this.generators)
        {
            generator.Generate(buffer.Slice(offset, length));
            offset += length;
        }
    }
}
