namespace Pico;

public ref struct SpanBitWriter
{
    private readonly Span<byte> buffer;
    private int bitPosition = 0;

    public SpanBitWriter(Span<byte> buffer, int bitOffset = 0)
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

    #region [ Set Internal ]

    private void SetByteInternal(int position, byte value, int bitsCount)
    {
        var intOff = position >> 3;
        var bitOff = position & 7;
        var bitSum = bitsCount + bitOff;

        if (bitSum <= 8)
        {
            //var bufferMask = (byte)(~(0xff >> bitOff) | (0xff >> bitSum));
            var valueMask = (byte)(0xff >> (8 - bitsCount));
            var bufferMask = (byte)~(valueMask << (8 - bitSum));
            buffer[intOff] = (byte)(buffer[intOff] & bufferMask |
                                     (value & valueMask) << (8 - bitSum));
        }
        else
        {
            //var valueMask = (byte)(0xff << (bitSum - 8));
            var valueMask = (byte)(((1 << (8 - bitOff)) - 1) << (bitSum - 8));
            var bufferMask = (byte)(0xff << (8 - bitOff));
            buffer[intOff] = (byte)(buffer[intOff] & bufferMask |
                                    (value & valueMask) >> (bitSum - 8));

            valueMask = (byte)((1 << (bitSum - 8)) - 1);
            bufferMask = (byte)((1 << (16 - bitSum)) - 1);
            buffer[intOff + 1] = (byte)(buffer[intOff + 1] & bufferMask |
                                        (value & valueMask) << (16 - bitSum));
        }
    }

    private void SetShortInternal(int position, ushort value, int bitsCount)
    {
        if (bitsCount <= 8)
            SetByteInternal(position, (byte)value, bitsCount);
        else
        {
            SetByteInternal(position, (byte) (value >> 8), bitsCount - 8);
            SetByteInternal(position + (bitsCount - 8), (byte) (value), 8);
        }
    }

    private void SetIntInternal(int position, uint value, int bitsCount)
    {
        if (bitsCount <= 16)
            SetShortInternal(position, (ushort)value, bitsCount);
        else
        {
            SetShortInternal(position, (ushort)(value >> 16), bitsCount - 16);
            SetShortInternal(position + (bitsCount - 16), (ushort)(value), 16);
        }
    }

    private void SetLongInternal(int position, ulong value, int bitsCount)
    {
        if (bitsCount <= 32)
            SetIntInternal(position, (uint)value, bitsCount);
        else
        {
            SetIntInternal(position, (uint)(value >> 32), bitsCount - 32);
            SetIntInternal(position + (bitsCount - 32), (uint)(value), 32);
        }
    }

    #endregion

    #region [ Try Write/Poke Bit ]

    public bool TryPokeBit(bool bit)
    {
        if (RemainingBits < 1)
        {
            return false;
        }

        SetByteInternal(BitPosition, bit ? (byte)1 : (byte)0, 1);
        return true;
    }

    public void PokeBit(bool bit)
    {
        if (!TryPokeBit(bit)) throw new EndOfStreamException();
    }

    public bool TryWriteBit(bool bit)
    {
        if (!TryPokeBit(bit)) return false;
        bitPosition++;
        return true;
    }

    public void WriteBit(bool bit)
    {
        if (!TryWriteBit(bit)) throw new EndOfStreamException();
    }

    #endregion

    #region [ Try Write/Poke Byte ]

    public bool TryPokeByte(byte value, int bitCount = 8)
    {
        if (bitCount <= 0) throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, "Must be positive");
        if (bitCount > 8) throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, "Must be less than or equals to 8");
        if (RemainingBits < bitCount) return false;

        SetByteInternal(BitPosition, value, bitCount);
        return true;
    }

    public void PokeByte(byte value, int bitCount = 8)
    {
        if (!TryPokeByte(value, bitCount)) throw new EndOfStreamException();
    }

    public bool TryWriteByte(byte value, int bitCount = 8)
    {
        if (!TryPokeByte(value, bitCount)) return false;
        bitPosition += bitCount;
        return true;
    }

    public void WriteByte(byte value, int bitCount = 8)
    {
        if (!TryWriteByte(value, bitCount)) throw new EndOfStreamException();
    }

    #endregion

    #region [ Try Write/Poke Short ]

    public bool TryPokeShort(ushort value, int bitCount = 16)
    {
        if (bitCount <= 0) throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, "Must be positive");
        if (bitCount > 16) throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, "Must be less than or equals to 16");
        if (RemainingBits < bitCount) return false;

        SetShortInternal(BitPosition, value, bitCount);
        return true;
    }

    public void PokeShort(ushort value, int bitCount = 16)
    {
        if (!TryPokeShort(value, bitCount)) throw new EndOfStreamException();
    }

    public bool TryWriteShort(ushort value, int bitCount = 16)
    {
        if (!TryPokeShort(value, bitCount)) return false;
        bitPosition += bitCount;
        return true;
    }

    public void WriteShort(ushort value, int bitCount = 16)
    {
        if (!TryWriteShort(value, bitCount)) throw new EndOfStreamException();
    }

    #endregion

    #region [ Try Write/Poke Int ]

    public bool TryPokeInt(uint value, int bitCount = 32)
    {
        if (bitCount <= 0) throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, "Must be positive");
        if (bitCount > 32) throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, "Must be less than or equals to 32");
        if (RemainingBits < bitCount) return false;

        SetIntInternal(BitPosition, value, bitCount);
        return true;
    }

    public void PokeInt(uint value, int bitCount = 32)
    {
        if (!TryPokeInt(value, bitCount)) throw new EndOfStreamException();
    }

    public bool TryWriteInt(uint value, int bitCount = 32)
    {
        if (!TryPokeInt(value, bitCount)) return false;
        bitPosition += bitCount;
        return true;
    }

    public void WriteInt(uint value, int bitCount = 32)
    {
        if (!TryWriteInt(value, bitCount)) throw new EndOfStreamException();
    }

    #endregion

    #region [ Try Write/Poke Long ]

    public bool TryPokeLong(ulong value, int bitCount = 64)
    {
        if (bitCount <= 0) throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, "Must be positive");
        if (bitCount > 64) throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, "Must be less than or equals to 64");
        if (RemainingBits < bitCount) return false;

        SetLongInternal(BitPosition, value, bitCount);
        return true;
    }

    public void PokeLong(ulong value, int bitCount = 64)
    {
        if (!TryPokeLong(value, bitCount)) throw new EndOfStreamException();
    }

    public bool TryWriteLong(ulong value, int bitCount = 64)
    {
        if (!TryPokeLong(value, bitCount)) return false;
        bitPosition += bitCount;
        return true;
    }

    public void WriteLong(ulong value, int bitCount = 64)
    {
        if (!TryWriteLong(value, bitCount)) throw new EndOfStreamException();
    }

    #endregion

}
