using System.Text;

// ReSharper disable InconsistentNaming

namespace Pico;

public static class Encodings
{
    public static readonly IBinaryStringEncoder ASCII =
        new EncodingBinaryStringEncoder(Encoding.ASCII);
    public static readonly IBinaryStringEncoder BigEndianUnicode =
        new EncodingBinaryStringEncoder(Encoding.BigEndianUnicode);
    public static readonly IBinaryStringEncoder Default =
        new EncodingBinaryStringEncoder(Encoding.Default);
    public static readonly IBinaryStringEncoder Latin1 =
        new EncodingBinaryStringEncoder(Encoding.Latin1);
    public static readonly IBinaryStringEncoder UTF32 =
        new EncodingBinaryStringEncoder(Encoding.UTF32);
    public static readonly IBinaryStringEncoder UTF8 =
        new EncodingBinaryStringEncoder(Encoding.UTF8);
    public static readonly IBinaryStringEncoder Unicode =
        new EncodingBinaryStringEncoder(Encoding.Unicode);

    public static readonly IBinaryStringEncoder Base64 =
        new Base64BinaryStringEncoder();

    public static readonly IBinaryStringEncoder HexUppercase =
        new HexBinaryStringEncoder(true);
    public static readonly IBinaryStringEncoder HexLowercase =
        new HexBinaryStringEncoder(false);

}
