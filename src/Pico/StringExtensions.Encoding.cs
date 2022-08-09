using System.Buffers;
using System.Globalization;
using System.Text;

namespace Pico;

partial class StringExtensions
{
    public static byte[] ToUtf8Encoding(this string text) =>
        Encodings.UTF8.GetBytes(text);
    public static string FromUtf8Encoding(this byte[] bytes) =>
        Encodings.UTF8.GetString(bytes);

    public static byte[] ToAsciiEncoding(this string text) =>
        Encodings.ASCII.GetBytes(text);
    public static string FromAsciiEncoding(this byte[] bytes) =>
        Encodings.ASCII.GetString(bytes);

    public static byte[] ToUnicodeEncoding(this string text) =>
        Encodings.Unicode.GetBytes(text);
    public static string FromUnicodeEncoding(this byte[] bytes) =>
        Encodings.Unicode.GetString(bytes);

    public static byte[] ToUtf32Encoding(this string text) =>
        Encodings.UTF32.GetBytes(text);
    public static string FromUtf32Encoding(this byte[] bytes) =>
        Encodings.UTF32.GetString(bytes);



    public static byte[] FromBase64Encoding(this string text) =>
        Encodings.Base64.GetBytes(text);
    public static string ToBase64Encoding(this byte[] bytes) =>
        Encodings.Base64.GetString(bytes);


    public static byte[] FromHexEncoding(this string text) =>
        Encodings.HexUppercase.GetBytes(text);
    public static string ToHexEncoding(this byte[] bytes, bool upperCase = false) =>
        upperCase
            ? Encodings.HexUppercase.GetString(bytes)
            : Encodings.HexLowercase.GetString(bytes);
}
