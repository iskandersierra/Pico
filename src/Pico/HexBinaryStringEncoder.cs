namespace Pico;

public class HexBinaryStringEncoder :
    IBinaryStringEncoder
{
    private readonly bool upperCase;
    private static readonly Dictionary<byte, char> HexLookupLower;
    private static readonly Dictionary<byte, char> HexLookupUpper;
    private static readonly Dictionary<char, byte> HexLookdown;

    static HexBinaryStringEncoder()
    {
        HexLookupLower = Enumerable.Range(0, 16)
            .ToDictionary(
                i => (byte)i,
                i => i.ToString("x").ToLower()[0]);
        
        HexLookupUpper = Enumerable.Range(0, 16)
            .ToDictionary(
                i => (byte)i,
                i => i.ToString("X").ToUpper()[0]);

        HexLookdown = new Dictionary<char, byte>(16);
        foreach (var i in Enumerable.Range(0, 10))
        {
            HexLookdown[(char) ('0' + i)] = (byte)i;
        }
        foreach (var i in Enumerable.Range(0, 6))
        {
            HexLookdown[(char) ('a' + i)] = (byte)(10 + i);
            HexLookdown[(char) ('A' + i)] = (byte)(10 + i);
        }
    }

    public HexBinaryStringEncoder(bool upperCase = false)
    {
        this.upperCase = upperCase;
    }

    public int GetByteCount(ReadOnlySpan<char> text) => (text.Length + 1) / 2;

    public int GetCharCount(ReadOnlySpan<byte> buffer) => buffer.Length * 2;

    public int GetBytes(ReadOnlySpan<char> text, Span<byte> buffer)
    {
        // TODO: Benchmark and optimize this method.

        var textLength = text.Length;
        if (text.Length == 0) return 0;

        var bytesCount = GetByteCount(text);
        if (bytesCount > buffer.Length)
            throw new ArgumentException($"Bytes buffer is too small. Expected at least {bytesCount} bytes, but found {buffer.Length}.");

        var writer = new SpanBitWriter(buffer);
        var j = 0;

        if (textLength % 2 == 1)
        {
            var c = text[0];
            if (!HexLookdown.TryGetValue(c, out var b))
                throw new ArgumentException($"Invalid hex character: {c}");
            writer.WriteByte(b, 4);
            j++;
        }

        for (; j < textLength; j += 2)
        {
            var c1 = text[j];
            var c2 = text[j + 1];
            if (!HexLookdown.TryGetValue(c1, out var b1))
                throw new ArgumentException($"Invalid hex character: {c1}");
            if (!HexLookdown.TryGetValue(c2, out var b2))
                throw new ArgumentException($"Invalid hex character: {c2}");
            writer.WriteByte((byte)((b1 << 4) | b2));
        }

        return bytesCount;
    }

    public int GetChars(ReadOnlySpan<byte> buffer, Span<char> text)
    {
        // TODO: Benchmark and optimize this method.

        var bufferLength = buffer.Length;
        if (bufferLength == 0) return 0;

        var charCount = GetCharCount(buffer);
        if (charCount > text.Length)
            throw new ArgumentException($"Text buffer is too small. Expected at least {charCount} chars, but found {text.Length}.");

        var lookup = upperCase ? HexLookupUpper : HexLookupLower;

        for (var (i, j) = (0, 0); i < bufferLength; i++, j+=2)
        {
            var value = buffer[i];
            text[j] = lookup[(byte)(value >> 4)];
            text[j + 1] = lookup[(byte)(value & 0xf)];
        }

        return charCount;
    }
}
