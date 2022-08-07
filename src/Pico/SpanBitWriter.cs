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
        if (bitCount > 8) throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, "Must be less than 9");
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

}
