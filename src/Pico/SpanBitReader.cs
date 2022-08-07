namespace Pico;

public ref struct SpanBitReader
{
    private readonly ReadOnlySpan<byte> buffer;
    private int intOffset = 0; // whole offset in bytes
    private int bitOffset = 0; // 0..7

    public SpanBitReader(ReadOnlySpan<byte> buffer, int bitOffset = 0)
    {
        if (bitOffset < 0) throw new ArgumentOutOfRangeException(nameof(bitOffset), bitOffset, "Must be non-negative");
        this.buffer = buffer;
        BitLength = buffer.Length * 8;
        BitPosition = bitOffset;
    }

    public int BitLength { get; }

    public int BytePosition
    {
        get => intOffset;
        set => BitPosition = value * 8;
    }

    public int BitPosition
    {
        get => intOffset * 8 + bitOffset;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, "Must be non-negative");
            if (value > BitLength) throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be less than bit length: {BitLength}");
            intOffset = value >> 3;
            bitOffset = value & 7;
        }
    }

    public int RemainingBits => BitLength - BitPosition;

    #region [ Try Read/Peek Bit ]

    public bool TryPeekBit(out bool bit)
    {
        if (RemainingBits < 1)
        {
            bit = false;
            return false;
        }

        bit = (buffer[intOffset] & (1 << (7 - bitOffset))) != 0;
        return true;
    }

    public bool PeekBit()
    {
        if (TryPeekBit(out var bit)) return bit;
        throw new EndOfStreamException();
    }

    public bool TryReadBit(out bool bit)
    {
        if (TryPeekBit(out bit))
        {
            bitOffset++;
            UpdateOffsets();
            return true;
        }
        return false;
    }

    public bool ReadBit()
    {
        if (TryReadBit(out var bit)) return bit;
        throw new EndOfStreamException();
    }

    #endregion

    #region [ Try Read/Peek Byte ]

    public bool TryPeekByte(int bitsCount, out byte value)
    {
        if (bitsCount <= 0) throw new ArgumentOutOfRangeException(nameof(bitsCount), bitsCount, "Must be positive");
        if (bitsCount > 8) throw new ArgumentOutOfRangeException(nameof(bitsCount), bitsCount, "Must be less than 8");
        if (bitsCount > RemainingBits)
        {
            value = 0;
            return false;
        }

        var bitSum = bitsCount + bitOffset;
        if (bitSum <= 8)
        {
            value = (byte)(buffer[intOffset] >> (8 - bitSum) & (byte)((1 << bitsCount) - 1));
            return true;
        }

        value = (byte)(buffer[intOffset] & ((1 << (8 - bitOffset)) - 1));
        value = (byte) ((value << (bitSum - 8)) |
                        (buffer[intOffset + 1] >> (16 - bitSum)));
        return true;
    }

    public bool TryPeekByte(out byte value) => TryPeekByte(8, out value);

    public byte PeekByte(int bitsCount = 8)
    {
        if (TryPeekByte(bitsCount, out var value)) return value;
        throw new EndOfStreamException();
    }

    public bool TryReadByte(int bitsCount, out byte value)
    {
        if (TryPeekByte(bitsCount, out value))
        {
            bitOffset += bitsCount;
            UpdateOffsets();
            return true;
        }
        return false;
    }

    public bool TryReadByte(out byte value) => TryReadByte(8, out value);

    public byte ReadByte(int bitsCount = 8)
    {
        if (TryReadByte(bitsCount, out var value)) return value;
        throw new EndOfStreamException();
    }

    #endregion

    private void UpdateOffsets()
    {
        intOffset += bitOffset >> 3;
        bitOffset &= 7;
    }
}
