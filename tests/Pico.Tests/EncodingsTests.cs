using System.Text;

namespace Pico.Tests;

public class EncodingsTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData(" ", "20")]
    [InlineData("Hello", "48656c6c6f")]
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
    [InlineData("Hello", "00480065006c006c006f")]
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
    [InlineData("Hello", "48656c6c6f")]
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
    [InlineData("Hello", "48656c6c6f")]
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
    [InlineData("Hello", "48000000650000006c0000006c0000006f000000")]
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
    [InlineData("Hello", "480065006c006c006f00")]
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
    [InlineData("Hello", "48656c6c6f")]
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
    [InlineData("AA==", "00")]
    [InlineData("AAA=", "0000")]
    [InlineData("AAAA", "000000")]
    [InlineData("IA==", "20")]
    [InlineData("ICA=", "2020")]
    [InlineData("ICAg", "202020")]
    [InlineData("SGVsbG8=", "48656c6c6f")]
    [InlineData("SGVsbG8h", "48656c6c6f21")]
    [InlineData("4bi8xqHhtonDq+G2hiDIi+G5lcWh4baZ4bmBIOG4jeG7oeG4vcet4bWz", "e1b8bcc6a1e1b689c3abe1b68620c88be1b995c5a1e1b699e1b98120e1b88de1bba1e1b8bdc7ade1b5b3")]
    public void Base64(string text, string hexBytes)
    {
        // GIVEN the encoding is UTF-8
        var encoding = Encodings.Base64;

        // THEN all tests apply
        RunTests(encoding, text, hexBytes);
    }

    [Theory]
    [InlineData("20")]
    [InlineData("2020")]
    [InlineData("202020")]
    [InlineData("48656c6c6f")]
    [InlineData("48656c6c6f21")]
    [InlineData("e1b8bcc6a1e1b689c3abe1b68620c88be1b995c5a1e1b699e1b98120e1b88de1bba1e1b8bdc7ade1b5b3")]
    public void HexLowercase(string hexBytes)
    {
        // GIVEN the encoding is UTF-8
        var encoding = Encodings.HexLowercase;

        // THEN all tests apply
        RunTests(encoding, hexBytes, hexBytes);
    }

    [Theory]
    [InlineData("20")]
    [InlineData("2020")]
    [InlineData("202020")]
    [InlineData("48656C6C6F21")]
    [InlineData("E1B8BCC6A1E1B689C3ABE1B68620C88BE1B995C5A1E1B699E1B98120E1B88DE1BBA1E1B8BDC7ADE1B5B3")]
    public void HexUppercase(string hexBytes)
    {
        // GIVEN the encoding is UTF-8
        var encoding = Encodings.HexUppercase;

        // THEN all tests apply
        RunTests(encoding, hexBytes, hexBytes);
    }

    private string ToHexEncoding(byte[] bytes)
    {
        var sb = new StringBuilder();
        foreach (var b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }

    private byte[] FromHexEncoding(string hex)
    {
        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < hex.Length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }

    private void RunTests(IBinaryStringEncoder encoder, string text, string hexBytes)
    {
        var bytes = FromHexEncoding(hexBytes);

        // WHEN the text is encoded to bytes
        var bytesBuffer = new byte[bytes.Length];
        var bytesCount = encoder.GetBytes(text.AsSpan(), bytesBuffer.AsSpan());

        // THEN the bytes are equal to the expected values
        ToHexEncoding(bytesBuffer).Should().BeEquivalentTo(hexBytes);

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
