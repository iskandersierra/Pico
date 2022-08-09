using System.Diagnostics;
using System.Text;

namespace Pico;

// TODO: Optimize to use Spans
public class Base64BinaryStringEncoder :
    IBinaryStringEncoder
{
    public int GetByteCount(ReadOnlySpan<char> text) =>
        Convert.FromBase64CharArray(
            text.ToArray(), 0, text.Length).Length;

    public int GetCharCount(ReadOnlySpan<byte> buffer) =>
        Convert.ToBase64String(
            buffer.ToArray(), 0, buffer.Length).Length;

    public int GetBytes(ReadOnlySpan<char> text, Span<byte> buffer)
    {
        var result = Convert.FromBase64CharArray(
            text.ToArray(), 0, text.Length);

        result.CopyTo(buffer);

        return result.Length;
    }

    public int GetChars(ReadOnlySpan<byte> buffer, Span<char> text)
    {
        var result = Convert.ToBase64String(
            buffer.ToArray(), 0, buffer.Length);

        result.AsSpan().CopyTo(text);

        return result.Length;
    }
}

//public class Base64BinaryStringEncoder :
//    IBinaryStringEncoder
//{
//    private const string DefaultVocabString =
//        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
//    private const char DefaultPaddingChar = '=';
//    private const char DefaultNewLineChar = '\n';

//    private static readonly char[] DefaultVocab;
//    private static readonly Dictionary<char, byte> DefaultInverseVocab;

//    static Base64BinaryStringEncoder()
//    {
//        (DefaultVocab, DefaultInverseVocab) = ProcessVocabulary(
//            DefaultVocabString, DefaultPaddingChar, DefaultNewLineChar);
//    }

//    private readonly char[] vocabulary;
//    private readonly Dictionary<char, byte> inverseVocabulary;
//    private readonly char padding;
//    private readonly char newLine;
//    private readonly int? maxLineLength;

//    public Base64BinaryStringEncoder(
//        string? vocabulary = null,
//        char padding = '=',
//        char newLine = '\n',
//        int? maxLineLength = null) // 
//    {
//        (this.vocabulary, inverseVocabulary) =
//            vocabulary is null
//                ? (DefaultVocab, DefaultInverseVocab)
//                : ProcessVocabulary(vocabulary, padding, newLine);
//        this.padding = padding;
//        this.newLine = newLine;
//        this.maxLineLength = maxLineLength;
//    }

//    public int GetByteCount(ReadOnlySpan<char> text)
//    {
//        var textLength = text.Length;
//        if (text.Length == 0) return 0;

//        var bitsCount = 0;
//        var lastCharHasValue = false;

//        for (var i = 0; i < textLength; i++)
//        {
//            var ch = text[i];
//            if (ch == padding) break;
//            if (inverseVocabulary.TryGetValue(ch, out var index))
//            {
//                bitsCount += 6;
//                var extraBits = bitsCount & 7;
//                lastCharHasValue = extraBits != 0 &&
//                                   (((1 << (bitsCount & 7)) - 1) & index) != 0;
//            }
//        }

//        var bytesCount = bitsCount >> 3;
//        if (lastCharHasValue) bytesCount++;
//        return bytesCount;
//    }

//    public int GetCharCount(ReadOnlySpan<byte> buffer)
//    {
//        return 4 * ((buffer.Length + 2) / 3);
//    }

//    public int GetBytes(ReadOnlySpan<char> text, Span<byte> buffer)
//    {
//        var textLength = text.Length;
//        if (text.Length == 0) return 0;

//        // TODO: Avoid using GetByteCount
//        var bytesCount = GetByteCount(text);
//        if (bytesCount > buffer.Length)
//            throw new ArgumentException($"Bytes buffer is too small. Expected at least {bytesCount} bytes, but found {buffer.Length}.");

//        var writer = new SpanBitWriter(buffer);
//        var remainingBits = bytesCount << 3;

//        for (var i = 0; i < textLength; i++)
//        {
//            var ch = text[i];
//            if (ch == padding) break;
//            if (inverseVocabulary.TryGetValue(ch, out var index))
//            {
//                var bitsToWrite = Math.Min(remainingBits, 6);
//                if (bitsToWrite == 0) break;
//                writer.WriteByte(index, bitsToWrite);
//                remainingBits -= bitsToWrite;
//            }
//        }

//        return bytesCount;
//    }

//    public int GetChars(ReadOnlySpan<byte> buffer, Span<char> text)
//    {
//        // TODO: Benchmark and optimize this method.

//        var bufferLength = buffer.Length;
//        if (bufferLength == 0) return 0;

//        var charCount = GetCharCount(buffer);
//        if (charCount > text.Length)
//            throw new ArgumentException($"Text buffer is too small. Expected at least {charCount} chars, but found {text.Length}.");

//        var bufferOverflow = bufferLength % 3;
//        var roundedBufferLength = bufferLength - bufferOverflow;

//        var byteIndex = 0;
//        var charIndex = 0;
//        while (byteIndex < roundedBufferLength)
//        {
//            var byte1 = buffer[byteIndex++];
//            var byte2 = buffer[byteIndex++];
//            var byte3 = buffer[byteIndex++];

//            var char1 = vocabulary[byte1 >> 2];
//            var char2 = vocabulary[((byte1 & 0x03) << 4) | (byte2 >> 4)];
//            var char3 = vocabulary[((byte2 & 0x0f) << 2) | (byte3 >> 6)];
//            var char4 = vocabulary[byte3 & 0x3f];

//            text[charIndex++] = char1;
//            text[charIndex++] = char2;
//            text[charIndex++] = char3;
//            text[charIndex++] = char4;
//        }

//        if (bufferOverflow == 1)
//        {
//            var byte1 = buffer[byteIndex++];

//            var char1 = vocabulary[byte1 >> 2];
//            var char2 = vocabulary[((byte1 & 0x03) << 4)];

//            text[charIndex++] = char1;
//            text[charIndex++] = char2;
//            text[charIndex++] = padding;
//            text[charIndex++] = padding;
//        }
//        else if (bufferOverflow == 2)
//        {
//            var byte1 = buffer[byteIndex++];
//            var byte2 = buffer[byteIndex++];

//            var char1 = vocabulary[byte1 >> 2];
//            var char2 = vocabulary[((byte1 & 0x03) << 4) | (byte2 >> 4)];
//            var char3 = vocabulary[((byte2 & 0x0f) << 2)];

//            text[charIndex++] = char1;
//            text[charIndex++] = char2;
//            text[charIndex++] = char3;
//            text[charIndex++] = padding;
//        }
//        else if (bufferOverflow == 0)
//        {
//            // do nothing
//        }

//        Debug.Assert(charIndex >= charCount);
//        Debug.Assert(byteIndex == bufferLength);

//        return charIndex;
//    }

//    private static (char[], Dictionary<char, byte>) ProcessVocabulary(
//        string vocabulary,
//        char padding,
//        char newLine)
//    {
//        if (vocabulary.Length != 64)
//        {
//            throw new ArgumentException(
//                $"The vocabulary must be 64 characters long. Found {vocabulary.Length}");
//        }

//        if (vocabulary.Contains(padding))
//        {
//            throw new ArgumentException(
//                $"The vocabulary must not contain the padding character '{padding}'");
//        }

//        if (vocabulary.Contains(newLine))
//        {
//            throw new ArgumentException(
//                $"The vocabulary must not contain the newline character '{newLine}'");
//        }

//        if (!vocabulary.AreDistinct())
//        {
//            throw new ArgumentException(
//                $"The vocabulary must not contain duplicate characters");
//        }

//        var vocabularyChars = vocabulary.ToCharArray();

//        var inverseVocabulary = new Dictionary<char, byte>();
//        for (byte i = 0; i < vocabularyChars.Length; i++)
//        {
//            inverseVocabulary.Add(vocabularyChars[i], i);
//        }

//        return (vocabularyChars, inverseVocabulary);
//    }
//}
