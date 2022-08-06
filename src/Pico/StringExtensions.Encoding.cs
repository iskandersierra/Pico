using System.Buffers;
using System.Globalization;
using System.Text;

namespace Pico;

partial class StringExtensions
{
    public static byte[] ToEncoding(this string text, Encoding encoding) => encoding.GetBytes(text);
    public static string FromEncoding(this byte[] bytes, Encoding encoding) => encoding.GetString(bytes);


    public static byte[] ToUtf8Encoding(this string text) => text.ToEncoding(Encoding.UTF8);
    public static string FromUtf8Encoding(this byte[] bytes) => bytes.FromEncoding(Encoding.UTF8);

    public static byte[] ToAsciiEncoding(this string text) => text.ToEncoding(Encoding.ASCII);
    public static string FromAsciiEncoding(this byte[] bytes) => bytes.FromEncoding(Encoding.ASCII);

    public static byte[] ToUnicodeEncoding(this string text) => text.ToEncoding(Encoding.Unicode);
    public static string FromUnicodeEncoding(this byte[] bytes) => bytes.FromEncoding(Encoding.Unicode);

    public static byte[] ToUtf32Encoding(this string text) => text.ToEncoding(Encoding.UTF32);
    public static string FromUtf32Encoding(this byte[] bytes) => bytes.FromEncoding(Encoding.UTF32);



    public static byte[] FromBase64Encoding(this string text) => Convert.FromBase64String(text);
    public static string ToBase64Encoding(this byte[] bytes) => Convert.ToBase64String(bytes);


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
