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
        if (RemainingBits < 0) throw new ArgumentOutOfRangeException(nameof(bitOffset), bitOffset, $"Must be less than buffer length: {buffer.Length * 8}");
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
            intOffset = value / 8;
            bitOffset = value % 8;
        }
    }

    public int RemainingBits => BitLength - BitPosition;

    public bool TryPeekBit(out bool bit)
    {
        if (RemainingBits < 1)
        {
            bit = false;
            return false;
        }

        bit = (buffer[intOffset] & (1 << bitOffset)) != 0;
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

    public bool TryPeekByte(int bitsCount, out byte value)
    {
        if (bitsCount <= 0) throw new ArgumentOutOfRangeException(nameof(bitsCount), bitsCount, "Must be positive");
        if (bitsCount > 8) throw new ArgumentOutOfRangeException(nameof(bitsCount), bitsCount, "Must be less than 8");
        if (bitsCount > RemainingBits)
        {
            value = 0;
            return false;
        }

        var bitCount1 = Math.Min(bitsCount, 8 - bitOffset);
        var extraBits = 8 - bitCount1 - bitOffset;
        var localValue = buffer[intOffset] >> extraBits;
        value = 0;
        return true;
    }

    private void UpdateOffsets()
    {
        intOffset += bitOffset >> 3;
        bitOffset &= 7;
    }
}
