using System.Diagnostics;
using System.Text;

namespace Pico;

public class Base64BinaryStringEncoder :
    IBinaryStringEncoder
{
    private const string DefaultVocabString =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    private const char DefaultPaddingChar = '=';
    private const char DefaultNewLineChar = '\n';

    private static readonly char[] DefaultVocab;
    private static readonly Dictionary<char, byte> DefaultInverseVocab;
    private static readonly string[] DefaultPaddings;

    static Base64BinaryStringEncoder()
    {
        (DefaultVocab, DefaultInverseVocab, DefaultPaddings) = ProcessVocabulary(
            DefaultVocabString, DefaultPaddingChar, DefaultNewLineChar);
    }

    private readonly char[] vocabulary;
    private readonly Dictionary<char, byte> inverseVocabulary;
    private readonly char padding;
    private readonly string[] paddings;
    private readonly char newLine;
    private readonly int? maxLineLength;

    public Base64BinaryStringEncoder(
        string? vocabulary = null,
        char padding = '=',
        char newLine = '\n',
        int? maxLineLength = null) // 
    {
        (this.vocabulary, inverseVocabulary, paddings) =
            vocabulary is null
                ? (DefaultVocab, DefaultInverseVocab, DefaultPaddings)
                : ProcessVocabulary(vocabulary, padding, newLine);
        this.padding = padding;
        this.newLine = newLine;
        this.maxLineLength = maxLineLength;
    }

    public int GetByteCount(ReadOnlySpan<char> text)
    {
        if (text.Length == 0) return 0;

        throw new NotImplementedException();
    }

    public int GetCharCount(ReadOnlySpan<byte> buffer) =>
        (buffer.Length + 2) / 3;

    public int GetBytes(ReadOnlySpan<char> text, Span<byte> buffer)
    {
        if (text.Length == 0) return 0;

        throw new NotImplementedException();
    }

    private int GetBytesAux(ReadOnlySpan<char> text, Action<int, byte> setByte)
    {
        throw new NotImplementedException();
        // TODO: Benchmark and optimize this method.
        //var textLength = text.Length;
        //if (textLength == 0) return 0;

        //var byteIndex = 0;
        //var charIndex = 0;
        //var currentChars = new char[4];
        //var currentBytes = new char[3];

        //while (charIndex < textLength)
        //{
        //    var validChars = 0;
        //    while (charIndex < textLength && validChars < 4)
        //    {
        //        var ch = text[charIndex];
        //        if (ch == padding) break;
        //        if (inverseVocabulary.TryGetValue(ch, out var b))
        //        {
        //            currentChars[validChars] = ch;
                    
        //            validChars++;
        //        }
        //        charIndex++;
        //    }

        //    if (validChars == 0) break;

        //}
    }

    public int GetChars(ReadOnlySpan<byte> buffer, Span<char> text)
    {
        // TODO: Benchmark and optimize this method.
        var bufferLength = buffer.Length;
        if (bufferLength == 0) return 0;

        var charCount = GetCharCount(buffer);
        if (charCount > text.Length)
            throw new ArgumentException($"Text buffer is too small. Expected at least {charCount} chars, but found {text.Length}.");

        var bufferOverflow = bufferLength % 3;
        var roundedBufferLength = bufferLength - bufferOverflow;

        var byteIndex = 0;
        var charIndex = 0;
        while (byteIndex < roundedBufferLength)
        {
            var byte1 = buffer[byteIndex++];
            var byte2 = buffer[byteIndex++];
            var byte3 = buffer[byteIndex++];

            var char1 = vocabulary[byte1 >> 2];
            var char2 = vocabulary[((byte1 & 0x03) << 4) | (byte2 >> 4)];
            var char3 = vocabulary[((byte2 & 0x0f) << 2) | (byte3 >> 6)];
            var char4 = vocabulary[byte3 & 0x3f];
            
            text[charIndex++] = char1;
            text[charIndex++] = char2;
            text[charIndex++] = char3;
            text[charIndex++] = char4;
        }

        if (bufferOverflow == 1)
        {
            var byte1 = buffer[byteIndex++];

            var char1 = vocabulary[byte1 >> 2];
            var char2 = vocabulary[((byte1 & 0x03) << 4)];

            text[charIndex++] = char1;
            text[charIndex++] = char2;
            text[charIndex++] = padding;
            text[charIndex++] = padding;
        }
        else if (bufferOverflow == 2)
        {
            var byte1 = buffer[byteIndex++];
            var byte2 = buffer[byteIndex++];

            var char1 = vocabulary[byte1 >> 2];
            var char2 = vocabulary[((byte1 & 0x03) << 4) | (byte2 >> 4)];
            var char3 = vocabulary[((byte2 & 0x0f) << 2)];

            text[charIndex++] = char1;
            text[charIndex++] = char2;
            text[charIndex++] = char3;
            text[charIndex++] = padding;
        }
        else if (bufferOverflow == 0)
        {
            // do nothing
        }

        Debug.Assert(charIndex == charCount);
        Debug.Assert(byteIndex == bufferLength);

        return charIndex;
    }

    private static (char[], Dictionary<char, byte>, string[]) ProcessVocabulary(
        string vocabulary,
        char padding,
        char newLine)
    {
        if (vocabulary.Length != 64)
        {
            throw new ArgumentException(
                $"The vocabulary must be 64 characters long. Found {vocabulary.Length}");
        }

        if (vocabulary.Contains(padding))
        {
            throw new ArgumentException(
                $"The vocabulary must not contain the padding character '{padding}'");
        }

        if (vocabulary.Contains(newLine))
        {
            throw new ArgumentException(
                $"The vocabulary must not contain the newline character '{newLine}'");
        }

        if (!vocabulary.AreDistinct())
        {
            throw new ArgumentException(
                $"The vocabulary must not contain duplicate characters");
        }

        var vocabularyChars = vocabulary.ToCharArray();

        var inverseVocabulary = new Dictionary<char, byte>();
        for (byte i = 0; i < vocabularyChars.Length; i++)
        {
            inverseVocabulary.Add(vocabularyChars[i], i);
        }

        var paddings = new []
        {
            "",
            new string(padding, 2),
            new string(padding, 1),
        };

        return (vocabularyChars, inverseVocabulary, paddings);
    }
}
