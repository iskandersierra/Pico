namespace Pico.Tests;

public partial class StringExtensionsTests
{
    [Theory]
    [InlineData("", new byte[0])]
    [InlineData(" ", new byte[] { 0x20 })]
    [InlineData("Hello", new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F })]
    [InlineData("Hello€", new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0xE2, 0x82, 0xAC })]
    public void ToUtf8Encoding(string text, byte[] utf8) =>
        text.ToUtf8Encoding().Should()
            .BeEquivalentTo(utf8, options => options.WithStrictOrdering());

    [Theory]
    [InlineData(new byte[0], "")]
    [InlineData(new byte[] { 0x20 }, " ")]
    [InlineData(new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F }, "Hello")]
    [InlineData(new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0xE2, 0x82, 0xAC }, "Hello€")]
    public void FromUtf8Encoding(byte[] utf8, string text) =>
        utf8.FromUtf8Encoding().Should().Be(text);



    [Theory]
    [InlineData("", new byte[0])]
    [InlineData(" ", new byte[] { 0x20 })]
    [InlineData("Hello", new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F })]
    [InlineData("Hello€", new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x3F })]
    public void ToAsciiEncoding(string text, byte[] utf8) =>
        text.ToAsciiEncoding().Should()
            .BeEquivalentTo(utf8, options => options.WithStrictOrdering());

    [Theory]
    [InlineData(new byte[0], "")]
    [InlineData(new byte[] { 0x20 }, " ")]
    [InlineData(new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F }, "Hello")]
    [InlineData(new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x3F }, "Hello?")]
    public void FromAsciiEncoding(byte[] utf8, string text) =>
        utf8.FromAsciiEncoding().Should().Be(text);



    [Theory]
    [InlineData("", new byte[0])]
    [InlineData(" ", new byte[] { 0x20, 0x00, 0x00, 0x00 })]
    [InlineData("Hello", new byte[] { 0x48, 0x00, 0x00, 0x00, 0x65, 0x00, 0x00, 0x00, 0x6C, 0x00, 0x00, 0x00, 0x6C, 0x00, 0x00, 0x00, 0x6F, 0x00, 0x00, 0x00 })]
    [InlineData("Hello€", new byte[] { 0x48, 0x00, 0x00, 0x00, 0x65, 0x00, 0x00, 0x00, 0x6C, 0x00, 0x00, 0x00, 0x6C, 0x00, 0x00, 0x00, 0x6F, 0x00, 0x00, 0x00, 0xAC, 0x20, 0x00, 0x00 })]
    public void ToUtf32Encoding(string text, byte[] utf8) =>
        text.ToUtf32Encoding().Should()
            .BeEquivalentTo(utf8, options => options.WithStrictOrdering());

    [Theory]
    [InlineData(new byte[0], "")]
    [InlineData(new byte[] { 0x20, 0x00, 0x00, 0x00 }, " ")]
    [InlineData(new byte[] { 0x48, 0x00, 0x00, 0x00, 0x65, 0x00, 0x00, 0x00, 0x6C, 0x00, 0x00, 0x00, 0x6C, 0x00, 0x00, 0x00, 0x6F, 0x00, 0x00, 0x00 }, "Hello")]
    [InlineData(new byte[] { 0x48, 0x00, 0x00, 0x00, 0x65, 0x00, 0x00, 0x00, 0x6C, 0x00, 0x00, 0x00, 0x6C, 0x00, 0x00, 0x00, 0x6F, 0x00, 0x00, 0x00, 0xAC, 0x20, 0x00, 0x00 }, "Hello€")]
    public void FromUtf32Encoding(byte[] utf8, string text) =>
        utf8.FromUtf32Encoding().Should().Be(text);



    [Theory]
    [InlineData("", new byte[0])]
    [InlineData(" ", new byte[] { 0x20, 0x00 })]
    [InlineData("Hello", new byte[] { 0x48, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x6C, 0x00, 0x6F, 0x00 })]
    [InlineData("Hello€", new byte[] { 0x48, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x6C, 0x00, 0x6F, 0x00, 0xAC, 0x20 })]
    public void ToUnicodeEncoding(string text, byte[] utf8) =>
        text.ToUnicodeEncoding().Should()
            .BeEquivalentTo(utf8, options => options.WithStrictOrdering());

    [Theory]
    [InlineData(new byte[0], "")]
    [InlineData(new byte[] { 0x20, 0x00 }, " ")]
    [InlineData(new byte[] { 0x48, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x6C, 0x00, 0x6F, 0x00 }, "Hello")]
    [InlineData(new byte[] { 0x48, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x6C, 0x00, 0x6F, 0x00, 0xAC, 0x20 }, "Hello€")]
    public void FromUnicodeEncoding(byte[] utf8, string text) =>
        utf8.FromUnicodeEncoding().Should().Be(text);



    [Theory]
    [InlineData("", new byte[0])]
    [InlineData("IA==", new byte[] { 0x20 })]
    [InlineData("SGVsbG8=", new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F })]
    public void FromBase64Encoding(string text, byte[] bytes) =>
        text.FromBase64Encoding().Should()
            .BeEquivalentTo(bytes, options => options.WithStrictOrdering());

    [Theory]
    [InlineData(new byte[0], "")]
    [InlineData(new byte[] { 0x20 }, "IA==")]
    [InlineData(new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F }, "SGVsbG8=")]
    public void ToBase64Encoding(byte[] bytes, string text) =>
        bytes.ToBase64Encoding().Should().Be(text);



    [Theory]
    [InlineData("", new byte[0])]
    [InlineData("0", new byte[] { 0x00 })]
    [InlineData("00", new byte[] { 0x00 })]
    [InlineData("20", new byte[] { 0x20 })]
    [InlineData("48656C6C6F", new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F })]
    [InlineData("48656C6c6f", new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F })]
    public void FromHexEncoding(string text, byte[] bytes) =>
        text.FromHexEncoding().Should()
            .BeEquivalentTo(bytes, options => options.WithStrictOrdering());

    [Theory]
    [InlineData(new byte[0], "")]
    [InlineData(new byte[] { 0x20 }, "20")]
    [InlineData(new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F }, "48656c6c6f")]
    public void ToHexEncoding(byte[] bytes, string text) =>
        bytes.ToHexEncoding().Should().Be(text);
}
