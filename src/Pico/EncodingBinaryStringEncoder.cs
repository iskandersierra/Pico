using System.Text;

namespace Pico;

public class EncodingBinaryStringEncoder :
    IBinaryStringEncoder
{
    private readonly Encoding encoding;

    public EncodingBinaryStringEncoder(Encoding encoding)
    {
        this.encoding = encoding;
    }

    public int GetByteCount(ReadOnlySpan<char> text) =>
        encoding.GetByteCount(text);

    public int GetCharCount(ReadOnlySpan<byte> buffer) =>
        encoding.GetCharCount(buffer);

    public int GetBytes(ReadOnlySpan<char> text, Span<byte> buffer)
    {
        var bytesCount = encoding.GetByteCount(text);

        if (bytesCount > buffer.Length)
        {
            throw new ArgumentException(
                $"The buffer is too small. Expected at least {bytesCount}, but found {buffer.Length}");
        }

        return encoding.GetBytes(text, buffer);
    }

    public int GetChars(ReadOnlySpan<byte> buffer, Span<char> text)
    {
        var charsCount = encoding.GetCharCount(buffer);

        if (charsCount > text.Length)
        {
            throw new ArgumentException(
                $"The text is too small. Expected at least {charsCount}, but found {text.Length}");
        }

        return encoding.GetChars(buffer, text);
    }
}
