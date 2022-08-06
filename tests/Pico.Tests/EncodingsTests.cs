namespace Pico.Tests;

public class EncodingsTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData(" ", "20")]
    [InlineData("Hello!", "48656c6c6f21")]
    public void ASCII(string text, string hexBytes)
    {
        // GIVEN the encoding is UTF-8
        var encoding = Encodings.ASCII;

        // THEN all tests apply
        RunTests(encoding, text, hexBytes);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(" ", "0020")]
    [InlineData("Hello!", "00480065006c006c006f0021")]
    [InlineData("Ḽơᶉëᶆ ȋṕšᶙṁ ḍỡḽǭᵳ", "1e3c01a11d8900eb1d860020020b1e5501611d991e4100201e0d1ee11e3d01ed1d73")]
    public void BigEndianUnicode(string text, string hexBytes)
    {
        // GIVEN the encoding is UTF-8
        var encoding = Encodings.BigEndianUnicode;

        // THEN all tests apply
        RunTests(encoding, text, hexBytes);
    }
    
    [Theory]
    [InlineData("", "")]
    [InlineData(" ", "20")]
    [InlineData("Hello!", "48656c6c6f21")]
    [InlineData("Ḽơᶉëᶆ ȋṕšᶙṁ ḍỡḽǭᵳ", "e1b8bcc6a1e1b689c3abe1b68620c88be1b995c5a1e1b699e1b98120e1b88de1bba1e1b8bdc7ade1b5b3")]
    public void Default(string text, string hexBytes)
    {
        // GIVEN the encoding is UTF-8
        var encoding = Encodings.Default;

        // THEN all tests apply
        RunTests(encoding, text, hexBytes);
    }
    
    [Theory]
    [InlineData("", "")]
    [InlineData(" ", "20")]
    [InlineData("Hello!", "48656c6c6f21")]
    public void Latin1(string text, string hexBytes)
    {
        // GIVEN the encoding is UTF-8
        var encoding = Encodings.Latin1;

        // THEN all tests apply
        RunTests(encoding, text, hexBytes);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(" ", "20000000")]
    [InlineData("Hello!", "48000000650000006c0000006c0000006f00000021000000")]
    [InlineData("Ḽơᶉëᶆ ȋṕšᶙṁ ḍỡḽǭᵳ", "3c1e0000a1010000891d0000eb000000861d0000200000000b020000551e000061010000991d0000411e0000200000000d1e0000e11e00003d1e0000ed010000731d0000")]
    public void UTF32(string text, string hexBytes)
    {
        // GIVEN the encoding is UTF-8
        var encoding = Encodings.UTF32;

        // THEN all tests apply
        RunTests(encoding, text, hexBytes);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(" ", "2000")]
    [InlineData("Hello!", "480065006c006c006f002100")]
    [InlineData("Ḽơᶉëᶆ ȋṕšᶙṁ ḍỡḽǭᵳ", "3c1ea101891deb00861d20000b02551e6101991d411e20000d1ee11e3d1eed01731d")]
    public void Unicode(string text, string hexBytes)
    {
        // GIVEN the encoding is UTF-8
        var encoding = Encodings.Unicode;

        // THEN all tests apply
        RunTests(encoding, text, hexBytes);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(" ", "20")]
    [InlineData("Hello!", "48656c6c6f21")]
    [InlineData("Ḽơᶉëᶆ ȋṕšᶙṁ ḍỡḽǭᵳ", "e1b8bcc6a1e1b689c3abe1b68620c88be1b995c5a1e1b699e1b98120e1b88de1bba1e1b8bdc7ade1b5b3")]
    public void UTF8(string text, string hexBytes)
    {
        // GIVEN the encoding is UTF-8
        var encoding = Encodings.UTF8;

        // THEN all tests apply
        RunTests(encoding, text, hexBytes);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(" ", "20")]
    [InlineData("Hello!", "48656c6c6f21")]
    [InlineData("Ḽơᶉëᶆ ȋṕšᶙṁ ḍỡḽǭᵳ", "e1b8bcc6a1e1b689c3abe1b68620c88be1b995c5a1e1b699e1b98120e1b88de1bba1e1b8bdc7ade1b5b3")]
    public void Base64(string text, string hexBytes)
    {
        // GIVEN the encoding is UTF-8
        var encoding = Encodings.Base64;

        // THEN all tests apply
        RunTests(encoding, text, hexBytes);
    }

    private void RunTests(IBinaryStringEncoder encoder, string text, string hexBytes)
    {
        var bytes = hexBytes.FromHexEncoding();

        // WHEN the text is encoded to bytes
        var bytesBuffer = new byte[bytes.Length];
        var bytesCount = encoder.GetBytes(text.AsSpan(), bytesBuffer.AsSpan());

        // THEN the bytes are equal to the expected values
        bytesBuffer.ToHexEncoding().Should().Be(hexBytes);

        // AND the bytes count is equal to the expected amount
        bytesCount.Should().Be(bytes.Length);

        // WHEN the bytes are encoded to text
        var textBuffer = new char[text.Length];
        var textCount = encoder.GetChars(bytes.AsSpan(), textBuffer.AsSpan());

        // THEN the text is equal to the expected values
        new string(textBuffer).Should().Be(text);

        // AND the text length is equal to the expected amount
        textCount.Should().Be(text.Length);

        // THEN the bytes count is equal to the expected amount
        encoder.GetByteCount(text.AsSpan()).Should().Be(bytes.Length);
        encoder.GetByteCount(text).Should().Be(bytes.Length);
        encoder.GetByteCount(text.ToCharArray()).Should().Be(bytes.Length);

        // AND the chars count is equal to the expected amount
        encoder.GetCharCount(bytes.AsSpan()).Should().Be(text.Length);
        encoder.GetCharCount(bytes).Should().Be(text.Length);

        if (bytes.Length > 0)
        {
            // WHEN bytes buffer is too small
            bytesBuffer = new byte[bytes.Length - 1];
            var encodeToBytes = () => encoder.GetBytes(text.AsSpan(), bytesBuffer.AsSpan());

            // THEN an argument exception is thrown
            encodeToBytes.Should().Throw<ArgumentException>();
        }

        if (text.Length > 0)
        {
            // WHEN text buffer is too small
            textBuffer = new char[text.Length - 1];
            var encodeToChars = () => encoder.GetChars(bytes.AsSpan(), textBuffer.AsSpan());

            // THEN an argument exception is thrown
            encodeToChars.Should().Throw<ArgumentException>();
        }
    }
}
