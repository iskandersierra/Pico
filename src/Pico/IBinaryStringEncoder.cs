namespace Pico;

public interface IBinaryStringEncoder
{
    int GetByteCount(ReadOnlySpan<char> text);

    int GetCharCount(ReadOnlySpan<byte> buffer);

    int GetBytes(ReadOnlySpan<char> text, Span<byte> buffer);

    int GetChars(ReadOnlySpan<byte> buffer, Span<char> text);

    public int GetByteCount(char[] text) => GetByteCount(text.AsSpan());
    public int GetByteCount(string text) => GetByteCount(text.AsSpan());

    public int GetCharCount(byte[] buffer) => GetCharCount(buffer.AsSpan());

    public int GetBytes(char[] text, byte[] buffer) =>
        GetBytes(text.AsSpan(), buffer.AsSpan());
    public int GetBytes(string text, byte[] buffer) =>
        GetBytes(text.AsSpan(), buffer.AsSpan());

    public byte[] GetBytes(char[] text)
    {
        var buffer = new byte[GetByteCount(text)];
        GetBytes(text, buffer);
        return buffer;
    }

    public byte[] GetBytes(string text)
    {
        var buffer = new byte[GetByteCount(text)];
        GetBytes(text, buffer);
        return buffer;
    }

    public int GetChars(byte[] buffer, char[] text) =>
        GetChars(buffer.AsSpan(), text.AsSpan());

    public char[] GetChars(byte[] buffer)
    {
        var text = new char[GetCharCount(buffer)];
        GetChars(buffer, text);
        return text;
    }

    public string GetString(byte[] buffer)
    {
        var text = new char[GetCharCount(buffer)];
        GetChars(buffer, text);
        return new string(text);
    }
}
