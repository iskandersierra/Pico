using System.Buffers;
using System.Globalization;
using System.Text;

namespace Pico;

partial class StringExtensions
{
    public static byte[] ToEncoding(this string text, Encoding encoding) => encoding.GetBytes(text);
    public static string FromEncoding(this byte[] bytes, Encoding encoding) => encoding.GetString(bytes);
    public static long ToEncoding(this ReadOnlySpan<char> text, Encoding encoding, IBufferWriter<byte> buffer) =>
        encoding.GetBytes(text, buffer);
    public static int ToEncoding(this ReadOnlySpan<char> text, Encoding encoding, Span<byte> bytes)
    {
        var length = encoding.GetByteCount(text);
        if (length > bytes.Length)
            throw new ArgumentException(
                $"The buffer is too small to hold the result. Expected: {length}, Actual: {bytes.Length}");
        return encoding.GetBytes(text, bytes);
    }
    public static long FromEncoding(this ReadOnlySpan<byte> bytes, Encoding encoding, IBufferWriter<char> buffer) =>
        encoding.GetChars(bytes, buffer);
    public static int FromEncoding(this ReadOnlySpan<byte> bytes, Encoding encoding, Span<char> chars)
    {
        var length = encoding.GetCharCount(bytes);
        if (length > chars.Length)
            throw new ArgumentException(
                $"The buffer is too small to hold the result. Expected: {length}, Actual: {chars.Length}");
        return encoding.GetChars(bytes, chars);
    }


    public static byte[] ToUtf8Encoding(this string text) => text.ToEncoding(Encoding.UTF8);
    public static string FromUtf8Encoding(this byte[] bytes) => bytes.FromEncoding(Encoding.UTF8);
    public static long ToUtf8Encoding(this ReadOnlySpan<char> text, IBufferWriter<byte> buffer) =>
        text.ToEncoding(Encoding.UTF8, buffer);
    public static int ToUtf8Encoding(this ReadOnlySpan<char> text, Span<byte> bytes) =>
        text.ToEncoding(Encoding.UTF8, bytes);
    public static long FromUtf8Encoding(this ReadOnlySpan<byte> bytes, IBufferWriter<char> buffer) =>
        bytes.FromEncoding(Encoding.UTF8, buffer);
    public static int FromUtf8Encoding(this ReadOnlySpan<byte> bytes, Span<char> chars) =>
        bytes.FromEncoding(Encoding.UTF8, chars);

    public static byte[] ToAsciiEncoding(this string text) => text.ToEncoding(Encoding.ASCII);
    public static string FromAsciiEncoding(this byte[] bytes) => bytes.FromEncoding(Encoding.ASCII);
    public static long ToAsciiEncoding(this ReadOnlySpan<char> text, IBufferWriter<byte> buffer) =>
        text.ToEncoding(Encoding.ASCII, buffer);
    public static int ToAsciiEncoding(this ReadOnlySpan<char> text, Span<byte> bytes) =>
        text.ToEncoding(Encoding.ASCII, bytes);
    public static long FromAsciiEncoding(this ReadOnlySpan<byte> bytes, IBufferWriter<char> buffer) =>
        bytes.FromEncoding(Encoding.ASCII, buffer);
    public static int FromAsciiEncoding(this ReadOnlySpan<byte> bytes, Span<char> chars) =>
        bytes.FromEncoding(Encoding.ASCII, chars);

    public static byte[] ToUnicodeEncoding(this string text) => text.ToEncoding(Encoding.Unicode);
    public static string FromUnicodeEncoding(this byte[] bytes) => bytes.FromEncoding(Encoding.Unicode);
    public static long ToUnicodeEncoding(this ReadOnlySpan<char> text, IBufferWriter<byte> buffer) =>
        text.ToEncoding(Encoding.Unicode, buffer);
    public static int ToUnicodeEncoding(this ReadOnlySpan<char> text, Span<byte> bytes) =>
        text.ToEncoding(Encoding.Unicode, bytes);
    public static long FromUnicodeEncoding(this ReadOnlySpan<byte> bytes, IBufferWriter<char> buffer) =>
        bytes.FromEncoding(Encoding.Unicode, buffer);
    public static int FromUnicodeEncoding(this ReadOnlySpan<byte> bytes, Span<char> chars) =>
        bytes.FromEncoding(Encoding.Unicode, chars);

    public static byte[] ToUtf32Encoding(this string text) => text.ToEncoding(Encoding.UTF32);
    public static string FromUtf32Encoding(this byte[] bytes) => bytes.FromEncoding(Encoding.UTF32);
    public static long ToUtf32Encoding(this ReadOnlySpan<char> text, IBufferWriter<byte> buffer) =>
        text.ToEncoding(Encoding.UTF32, buffer);
    public static int ToUtf32Encoding(this ReadOnlySpan<char> text, Span<byte> bytes) =>
        text.ToEncoding(Encoding.UTF32, bytes);
    public static long FromUtf32Encoding(this ReadOnlySpan<byte> bytes, IBufferWriter<char> buffer) =>
        bytes.FromEncoding(Encoding.UTF32, buffer);
    public static int FromUtf32Encoding(this ReadOnlySpan<byte> bytes, Span<char> chars) =>
        bytes.FromEncoding(Encoding.UTF32, chars);



    public static byte[] FromBase64Encoding(this string text) => Convert.FromBase64String(text);
    public static string ToBase64Encoding(this byte[] bytes) => Convert.ToBase64String(bytes);
    public static int FromBase64Encoding(this ReadOnlySpan<char> text, Span<byte> bytes)
    {
        if (Convert.TryFromBase64Chars(text, bytes, out var result)) return result;
        throw new ArgumentException("The input is not a valid Base64 string.");
    }
    public static int ToBase64Encoding(this ReadOnlySpan<byte> bytes, Span<char> text)
    {
        if (Convert.TryToBase64Chars(bytes, text, out var result)) return result;
        throw new ArgumentException("The input is not a valid Base64 string.");
    }


    public static byte[] FromHexEncoding(this string text)
    {
        if (text.IsNullOrEmpty()) return Array.Empty<byte>();

        var odd = text.Length % 2 == 1;

        var result = new byte[text.Length / 2 + (odd ? 1 : 0)];

        for (var i = text.Length; i > 0; i -= 2)
        {
            if (odd && i == 1)
            {
                result[0] = byte.Parse(
                    text.AsSpan(0, 1),
                    NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture);
            }
            else
            {
                result[(i - 1) / 2] = byte.Parse(
                    text.AsSpan(i - 2, 2),
                    NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture);
            }
        }

        return result;
    }
    public static string ToHexEncoding(this byte[] bytes, bool upperCase = false)
    {
        var sb = new StringBuilder();

        var format = upperCase ? "X2" : "x2";

        for (var i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i].ToString(format));
        }

        return sb.ToString();
    }
}
