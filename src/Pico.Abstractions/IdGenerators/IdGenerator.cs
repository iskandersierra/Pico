namespace Pico.Abstractions.IdGenerators;

public static class IdGenerator
{
    public static IIdGenerator FromBinary(
        IBinaryIdGenerator binaryGenerator,
        IBinaryStringEncoder encoder,
        int idLength)
    {
        return new BinaryIdGenerator(binaryGenerator, encoder, idLength);
    }

    class BinaryIdGenerator : IIdGenerator
    {
        private readonly IBinaryIdGenerator binaryGenerator;
        private readonly IBinaryStringEncoder encoder;

        public BinaryIdGenerator(
            IBinaryIdGenerator binaryGenerator,
            IBinaryStringEncoder encoder,
            int idLength)
        {
            this.binaryGenerator = binaryGenerator;
            this.encoder = encoder;
            this.IdLength = idLength;
        }

        public int IdLength { get; }

        public string Generate()
        {
            var bytes = binaryGenerator.Generate();
            return encoder.GetString(bytes);
        }
    }
}
