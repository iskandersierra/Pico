namespace Pico.Abstractions.IdGenerators;

public static class IdGenerator
{
    public static IIdGenerator FromBinary(
        IBinaryIdGenerator binaryGenerator,
        IBinaryStringEncoder encoder,
        int idLength) =>
        new BinaryIdGenerator(binaryGenerator, encoder, idLength);

    public static IIdGenerator CryptoOnBase64(
        int idLength)
    {
        IBinaryIdGenerator idGen = new CryptoBinaryIdGenerator(idLength);
        var encoder = Encodings.Base64;
        var sampleBytes = idGen.Generate();
        var charCount = encoder.GetCharCount(sampleBytes);
        return FromBinary(idGen, encoder, charCount);
    }

    public static IIdGenerator Guid(string format = "N") =>
        new GuidIdGenerator(format);

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

    class GuidIdGenerator : IIdGenerator
    {
        private readonly string format;

        public GuidIdGenerator(string format = "N")
        {
            switch (format)
            {
                case "N":
                    IdLength = 32;
                    break;
                case "D":
                    IdLength = 36;
                    break;
                case "B":
                case "P":
                    IdLength = 38;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"Expected one of the following formats ('N', 'D', 'B', 'P'), but '{format}' was found");
            }

            this.format = format;
        }

        public int IdLength { get; }

        public string Generate() => System.Guid.NewGuid().ToString(format);
    }
}
