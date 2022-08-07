namespace Pico;

public ref struct SpanBitReader
{
    private readonly ReadOnlySpan<byte> buffer;
    private int bitPosition = 0;

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
        get => bitPosition >> 3;
        set => BitPosition = value << 3;
    }

    public int BitPosition
    {
        get => bitPosition;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, "Must be non-negative");
            if (value > BitLength) throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be less than bit length: {BitLength}");
            bitPosition = value;
        }
    }

    public int BitOffset
    {
        get => bitPosition & 7;
    }

    public int RemainingBits => BitLength - BitPosition;

    #region [ Get Internal ]

    private byte GetByteInternal(int position, int bitsCount)
    {
        var intOff = position >> 3;
        var bitOff = position & 7;
        var bitSum = bitsCount + bitOff;
        if (bitSum <= 8)
            return (byte)(buffer[intOff] >> (8 - bitSum) & (byte)((1 << bitsCount) - 1));

        var value = (byte)(buffer[intOff] & ((1 << (8 - bitOff)) - 1));
        value = (byte)((value << (bitSum - 8)) |
                       (buffer[intOff + 1] >> (16 - bitSum)));
        return value;
    }

    private ushort GetShortInternal(int position, int bitsCount)
    {
        if (bitsCount <= 8)
            return GetByteInternal(position, bitsCount);

        return (ushort)((ushort)(GetByteInternal(position, 8) << (bitsCount - 8)) |
                        GetByteInternal(position + 8, bitsCount - 8));
    }

    private uint GetIntInternal(int position, int bitsCount)
    {
        if (bitsCount <= 16)
            return GetShortInternal(position, bitsCount);

        return (uint)GetShortInternal(position, 16) << (bitsCount - 16) |
               GetShortInternal(position + 16, bitsCount - 16);
    }

    private ulong GetLongInternal(int position, int bitsCount)
    {
        if (bitsCount <= 32)
            return GetIntInternal(position, bitsCount);

        return (ulong)GetIntInternal(position, 32) << (bitsCount - 32) |
               GetIntInternal(position + 32, bitsCount - 32);
    }

    #endregion

    #region [ Try Read/Peek Bit ]

    public bool TryPeekBit(out bool bit)
    {
        if (RemainingBits < 1)
        {
            bit = false;
            return false;
        }

        bit = GetByteInternal(bitPosition, 1) != 0;
        return true;
    }

    public bool PeekBit()
    {
        if (TryPeekBit(out var bit)) return bit;
        throw new EndOfStreamException();
    }

    public bool TryReadBit(out bool bit)
    {
        if (!TryPeekBit(out bit)) return false;
        bitPosition++;
        return true;
    }

    public bool ReadBit()
    {
        if (TryReadBit(out var bit)) return bit;
        throw new EndOfStreamException();
    }

    #endregion

    #region [ Try Read/Peek Byte ]

    public bool TryPeekByte(out byte value, int bitsCount = 8)
    {
        if (bitsCount <= 0) throw new ArgumentOutOfRangeException(nameof(bitsCount), bitsCount, "Must be positive");
        if (bitsCount > 8) throw new ArgumentOutOfRangeException(nameof(bitsCount), bitsCount, "Must be less than 9");
        if (bitsCount > RemainingBits)
        {
            value = 0;
            return false;
        }

        value = GetByteInternal(BitPosition, bitsCount);
        return true;
    }

    public byte PeekByte(int bitsCount = 8)
    {
        if (TryPeekByte(out var value, bitsCount)) return value;
        throw new EndOfStreamException();
    }

    public bool TryReadByte(out byte value, int bitsCount = 8)
    {
        if (!TryPeekByte(out value, bitsCount)) return false;
        bitPosition += bitsCount;
        return true;
    }

    public byte ReadByte(int bitsCount = 8)
    {
        if (TryReadByte(out var value, bitsCount)) return value;
        throw new EndOfStreamException();
    }

    #endregion

    #region [ Try Read/Peek Short ]

    public bool TryPeekShort(out ushort value, int bitsCount = 16)
    {
        if (bitsCount <= 0) throw new ArgumentOutOfRangeException(nameof(bitsCount), bitsCount, "Must be positive");
        if (bitsCount > 16) throw new ArgumentOutOfRangeException(nameof(bitsCount), bitsCount, "Must be less than 17");
        if (bitsCount > RemainingBits)
        {
            value = 0;
            return false;
        }

        value = GetShortInternal(BitPosition, bitsCount);
        return true;
    }

    public ushort PeekShort(int bitsCount = 16)
    {
        if (TryPeekShort(out var value, bitsCount)) return value;
        throw new EndOfStreamException();
    }

    public bool TryReadShort(out ushort value, int bitsCount = 16)
    {
        if (!TryPeekShort(out value, bitsCount)) return false;
        bitPosition += bitsCount;
        return true;
    }

    public ushort ReadShort(int bitsCount = 16)
    {
        if (TryReadShort(out var value, bitsCount)) return value;
        throw new EndOfStreamException();
    }

    #endregion

    #region [ Try Read/Peek Int ]

    public bool TryPeekInt(out uint value, int bitsCount = 32)
    {
        if (bitsCount <= 0) throw new ArgumentOutOfRangeException(nameof(bitsCount), bitsCount, "Must be positive");
        if (bitsCount > 32) throw new ArgumentOutOfRangeException(nameof(bitsCount), bitsCount, "Must be less than 33");
        if (bitsCount > RemainingBits)
        {
            value = 0;
            return false;
        }

        value = GetIntInternal(BitPosition, bitsCount);
        return true;
    }

    public uint PeekInt(int bitsCount = 32)
    {
        if (TryPeekInt(out var value, bitsCount)) return value;
        throw new EndOfStreamException();
    }

    public bool TryReadInt(out uint value, int bitsCount = 32)
    {
        if (!TryPeekInt(out value, bitsCount)) return false;
        bitPosition += bitsCount;
        return true;
    }

    public uint ReadInt(int bitsCount = 32)
    {
        if (TryReadInt(out var value, bitsCount)) return value;
        throw new EndOfStreamException();
    }

    #endregion

    #region [ Try Read/Peek Long ]

    public bool TryPeekLong(out ulong value, int bitsCount = 64)
    {
        if (bitsCount <= 0) throw new ArgumentOutOfRangeException(nameof(bitsCount), bitsCount, "Must be positive");
        if (bitsCount > 64) throw new ArgumentOutOfRangeException(nameof(bitsCount), bitsCount, "Must be less than 65");
        if (bitsCount > RemainingBits)
        {
            value = 0;
            return false;
        }

        value = GetLongInternal(BitPosition, bitsCount);
        return true;
    }

    public ulong PeekLong(int bitsCount = 64)
    {
        if (TryPeekLong(out var value, bitsCount)) return value;
        throw new EndOfStreamException();
    }

    public bool TryReadLong(out ulong value, int bitsCount = 64)
    {
        if (!TryPeekLong(out value, bitsCount)) return false;
        bitPosition += bitsCount;
        return true;
    }

    public ulong ReadLong(int bitsCount = 64)
    {
        if (TryReadLong(out var value, bitsCount)) return value;
        throw new EndOfStreamException();
    }

    #endregion
}
