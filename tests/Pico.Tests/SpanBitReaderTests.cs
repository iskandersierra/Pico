namespace Pico.Tests;

public class SpanBitReaderTests
{

    #region [ Create ]

    [Theory]
    [InlineData(4, -1)]
    [InlineData(4, 33)]
    public void CreateWithBitOffsetOutOfRange(
        int bufferLength, int bitOffset)
    {
        // GIVEN a buffer
        var buffer = new byte[bufferLength];

        // WHEN the bit reader is created with a negative bit offset
        var action = () =>
        {
            new SpanBitReader(buffer, bitOffset);
        };

        // THEN an argument exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(4, 0, 0, 32)]
    [InlineData(4, 1, 0, 31)]
    [InlineData(4, 7, 0, 25)]
    [InlineData(4, 8, 1, 24)]
    [InlineData(4, 9, 1, 23)]
    [InlineData(4, 15, 1, 17)]
    [InlineData(4, 16, 2, 16)]
    [InlineData(4, 17, 2, 15)]
    [InlineData(4, 23, 2, 9)]
    [InlineData(4, 24, 3, 8)]
    [InlineData(4, 25, 3, 7)]
    [InlineData(4, 31, 3, 1)]
    [InlineData(4, 32, 4, 0)]
    public void Create(
        int bufferLength, int bitOffset,
        int expectedBytePosition, int expectedRemainingBits)
    {
        // GIVEN a buffer
        var buffer = new byte[bufferLength];

        // WHEN a bit reader is created
        var bitReader = new SpanBitReader(buffer, bitOffset);

        // THEN the bit reader properties are as expected
        bitReader.BitLength.Should().Be(bufferLength * 8);
        bitReader.BytePosition.Should().Be(expectedBytePosition);
        bitReader.BitPosition.Should().Be(bitOffset);
        bitReader.RemainingBits.Should().Be(expectedRemainingBits);
    }

    #endregion

    #region [ SetPosition ]

    [Theory]
    [InlineData(4, -1)]
    [InlineData(4, 33)]
    public void SetBitPositionOutOfRange(
        int bufferLength, int invalidBitPosition)
    {
        // GIVEN a buffer
        var buffer = new byte[bufferLength];

        // WHEN the bit reader is created and set to an invalid bit position
        var action = () =>
        {
            var bitReader = new SpanBitReader(buffer);
            return bitReader.BitPosition = invalidBitPosition;
        };

        // THEN an argument exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(4, 0, 0, 32)]
    [InlineData(4, 1, 0, 31)]
    [InlineData(4, 7, 0, 25)]
    [InlineData(4, 8, 1, 24)]
    [InlineData(4, 9, 1, 23)]
    [InlineData(4, 15, 1, 17)]
    [InlineData(4, 16, 2, 16)]
    [InlineData(4, 17, 2, 15)]
    [InlineData(4, 23, 2, 9)]
    [InlineData(4, 24, 3, 8)]
    [InlineData(4, 25, 3, 7)]
    [InlineData(4, 31, 3, 1)]
    [InlineData(4, 32, 4, 0)]
    public void SetBitPosition(
        int bufferLength, int bitOffset,
        int expectedBytePosition, int expectedRemainingBits)
    {
        // GIVEN a buffer
        var buffer = new byte[bufferLength];

        // GIVEN a bit reader
        var bitReader = new SpanBitReader(buffer);

        // WHEN the bit reader is set to a valid bit position
        bitReader.BitPosition = bitOffset;

        // THEN the bit reader properties are as expected
        bitReader.BitLength.Should().Be(bufferLength * 8);
        bitReader.BytePosition.Should().Be(expectedBytePosition);
        bitReader.BitPosition.Should().Be(bitOffset);
        bitReader.RemainingBits.Should().Be(expectedRemainingBits);
    }

    [Theory]
    [InlineData(4, -1)]
    [InlineData(4, 5)]
    public void SetBytePositionOutOfRange(
        int bufferLength, int invalidBytePosition)
    {
        // GIVEN a buffer
        var buffer = new byte[bufferLength];

        // WHEN the bit reader is created and set to an invalid byte position
        var action = () =>
        {
            var bitReader = new SpanBitReader(buffer);
            return bitReader.BytePosition = invalidBytePosition;
        };

        // THEN an argument exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(4, 0, 0, 32)]
    [InlineData(4, 1, 1, 24)]
    [InlineData(4, 2, 2, 16)]
    [InlineData(4, 3, 3, 8)]
    [InlineData(4, 4, 4, 0)]
    public void SetBytePosition(
        int bufferLength, int bytePosition,
        int expectedBytePosition, int expectedRemainingBits)
    {
        // GIVEN a buffer
        var buffer = new byte[bufferLength];

        // GIVEN a bit reader
        var bitReader = new SpanBitReader(buffer);

        // WHEN the bit reader is set to a valid bit position
        bitReader.BytePosition = bytePosition;

        // THEN the bit reader properties are as expected
        bitReader.BitLength.Should().Be(bufferLength * 8);
        bitReader.BytePosition.Should().Be(expectedBytePosition);
        bitReader.BitPosition.Should().Be(bytePosition * 8);
        bitReader.RemainingBits.Should().Be(expectedRemainingBits);
    }

    #endregion

    #region [ Try Read/Peek Bit ]

    [Theory]
    [InlineData("", 0, false, false)]
    [InlineData("20", 0, true, false)]
    [InlineData("20", 2, true, true)]
    [InlineData("20", 7, true, false)]
    [InlineData("20", 8, false, false)]
    [InlineData("2040", 0, true, false)]
    [InlineData("2040", 2, true, true)]
    [InlineData("2040", 7, true, false)]
    [InlineData("2040", 8, true, false)]
    [InlineData("2040", 9, true, true)]
    [InlineData("2040", 15, true, false)]
    [InlineData("2040", 16, false, false)]
    public void TryPeekBit(
        string hexBytes, int bitOffset,
        bool expectedResult, bool expectedBit)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        var bitReader = new SpanBitReader(buffer, bitOffset);

        // WHEN a bit is tried to be peek
        var result = bitReader.TryPeekBit(out var bit);

        // THEN the result is as expected
        result.Should().Be(expectedResult);

        // AND the bit is as expected
        bit.Should().Be(expectedBit);

        // AND the bit position is unchanged
        bitReader.BitPosition.Should().Be(bitOffset);
    }

    [Theory]
    [InlineData("", 0, false, false)]
    [InlineData("20", 0, true, false)]
    [InlineData("20", 2, true, true)]
    [InlineData("20", 7, true, false)]
    [InlineData("20", 8, false, false)]
    [InlineData("2040", 0, true, false)]
    [InlineData("2040", 2, true, true)]
    [InlineData("2040", 7, true, false)]
    [InlineData("2040", 8, true, false)]
    [InlineData("2040", 9, true, true)]
    [InlineData("2040", 15, true, false)]
    [InlineData("2040", 16, false, false)]
    public void PeekBit(
        string hexBytes, int bitOffset,
        bool expectedResult, bool expectedBit)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        if (expectedResult)
        {
            // GIVEN a bit reader
            var bitReader = new SpanBitReader(buffer, bitOffset);

            // WHEN a bit is peeked
            var bit = bitReader.PeekBit();

            // THEN the bit is as expected
            bit.Should().Be(expectedBit);

            // THEN the bit position is unchanged
            bitReader.BitPosition.Should().Be(bitOffset);
        }
        else
        {
            // GIVEN a bit reader
            // WHEN a bit is peeked
            var action = () =>
            {
                var bitReader = new SpanBitReader(buffer, bitOffset);
                return bitReader.PeekBit();
            };

            // THEN an exception is thrown
            action.Should().Throw<EndOfStreamException>();
        }
    }

    [Theory]
    [InlineData("", 0, false, false)]
    [InlineData("20", 0, true, false)]
    [InlineData("20", 2, true, true)]
    [InlineData("20", 7, true, false)]
    [InlineData("20", 8, false, false)]
    [InlineData("2040", 0, true, false)]
    [InlineData("2040", 2, true, true)]
    [InlineData("2040", 7, true, false)]
    [InlineData("2040", 8, true, false)]
    [InlineData("2040", 9, true, true)]
    [InlineData("2040", 15, true, false)]
    [InlineData("2040", 16, false, false)]
    public void TryReadBit(
        string hexBytes, int bitOffset,
        bool expectedResult, bool expectedBit)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        var bitReader = new SpanBitReader(buffer, bitOffset);

        // WHEN a bit is tried to be peek
        var result = bitReader.TryReadBit(out var bit);

        // THEN the result is as expected
        result.Should().Be(expectedResult);

        // THEN the bit is as expected
        bit.Should().Be(expectedBit);

        if (expectedResult)
        {
            // THEN the bit position is increased by 1
            bitReader.BitPosition.Should().Be(bitOffset + 1);
        }
        else
        {
            // THEN the bit position is unchanged
            bitReader.BitPosition.Should().Be(bitOffset);
        }
    }

    [Theory]
    [InlineData("", 0, false, false)]
    [InlineData("20", 0, true, false)]
    [InlineData("20", 2, true, true)]
    [InlineData("20", 7, true, false)]
    [InlineData("20", 8, false, false)]
    [InlineData("2040", 0, true, false)]
    [InlineData("2040", 2, true, true)]
    [InlineData("2040", 7, true, false)]
    [InlineData("2040", 8, true, false)]
    [InlineData("2040", 9, true, true)]
    [InlineData("2040", 15, true, false)]
    [InlineData("2040", 16, false, false)]
    public void ReadBit(
        string hexBytes, int bitOffset,
        bool expectedResult, bool expectedBit)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        if (expectedResult)
        {
            // GIVEN a bit reader
            var bitReader = new SpanBitReader(buffer, bitOffset);

            // WHEN a bit is peeked
            var bit = bitReader.ReadBit();

            // THEN the bit is as expected
            bit.Should().Be(expectedBit);

            // THEN the bit position is increased by 1
            bitReader.BitPosition.Should().Be(bitOffset + 1);
        }
        else
        {
            // GIVEN a bit reader
            // WHEN a bit is peeked
            var action = () =>
            {
                var bitReader = new SpanBitReader(buffer, bitOffset);
                return bitReader.ReadBit();
            };

            // THEN an exception is thrown
            action.Should().Throw<EndOfStreamException>();
        }
    }

    #endregion

    #region [ Try Read/Peek Byte ]

    [Theory]
    [InlineData("20", 0, -1)]
    [InlineData("20", 0, 9)]
    public void TryPeekByteInvalidBitCount(
        string hexBytes, int bitOffset, int bitCount)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        // WHEN a byte is tried to be peek with invalid bit count
        var action = () =>
        {
            var bitReader = new SpanBitReader(buffer, bitOffset);
            bitReader.TryPeekByte(bitCount, out var value);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 1, 7, true, 0x27)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 3, 5, true, 0x07)]
    [InlineData("A7", 4, 4, true, 0x07)]
    [InlineData("A7", 5, 3, true, 0x07)]
    [InlineData("A7", 6, 2, true, 0x03)]
    [InlineData("A7", 7, 1, true, 0x01)]
    [InlineData("A7", 5, 4, false, 0)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 1, 7, true, 0x27)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 3, 5, true, 0x07)]
    [InlineData("A76B", 4, 4, true, 0x07)]
    [InlineData("A76B", 5, 3, true, 0x07)]
    [InlineData("A76B", 6, 2, true, 0x03)]
    [InlineData("A76B", 7, 1, true, 0x01)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 2, 8, true, 0x9D)]
    [InlineData("A76B", 3, 8, true, 0x3B)]
    [InlineData("A76B", 4, 8, true, 0x76)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 6, 8, true, 0xDA)]
    [InlineData("A76B", 7, 8, true, 0xB5)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 10, 6, true, 0x2B)]
    [InlineData("A76B", 11, 5, true, 0x0B)]
    [InlineData("A76B", 12, 4, true, 0x0B)]
    [InlineData("A76B", 13, 3, true, 0x03)]
    [InlineData("A76B", 14, 2, true, 0x03)]
    [InlineData("A76B", 15, 1, true, 0x01)]
    [InlineData("A76B", 13, 4, false, 0)]
    public void TryPeekByte(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, byte expectedByte)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        var bitReader = new SpanBitReader(buffer, bitOffset);

        // WHEN a byte is tried to be peek
        var result = bitCount == 8
            ? bitReader.TryPeekByte(out var value)
            : bitReader.TryPeekByte(bitCount, out value);

        // THEN the result is as expected
        result.Should().Be(expectedResult);

        // THEN the byte is as expected
        value.Should().Be(expectedByte);

        // THEN the bit position is unchanged
        bitReader.BitPosition.Should().Be(bitOffset);
    }
    
    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 1, 7, true, 0x27)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 3, 5, true, 0x07)]
    [InlineData("A7", 4, 4, true, 0x07)]
    [InlineData("A7", 5, 3, true, 0x07)]
    [InlineData("A7", 6, 2, true, 0x03)]
    [InlineData("A7", 7, 1, true, 0x01)]
    [InlineData("A7", 5, 4, false, 0)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 1, 7, true, 0x27)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 3, 5, true, 0x07)]
    [InlineData("A76B", 4, 4, true, 0x07)]
    [InlineData("A76B", 5, 3, true, 0x07)]
    [InlineData("A76B", 6, 2, true, 0x03)]
    [InlineData("A76B", 7, 1, true, 0x01)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 2, 8, true, 0x9D)]
    [InlineData("A76B", 3, 8, true, 0x3B)]
    [InlineData("A76B", 4, 8, true, 0x76)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 6, 8, true, 0xDA)]
    [InlineData("A76B", 7, 8, true, 0xB5)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 10, 6, true, 0x2B)]
    [InlineData("A76B", 11, 5, true, 0x0B)]
    [InlineData("A76B", 12, 4, true, 0x0B)]
    [InlineData("A76B", 13, 3, true, 0x03)]
    [InlineData("A76B", 14, 2, true, 0x03)]
    [InlineData("A76B", 15, 1, true, 0x01)]
    [InlineData("A76B", 13, 4, false, 0)]
    public void PeekByte(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, byte expectedByte)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        if (expectedResult)
        {
            // GIVEN a bit reader
            var bitReader = new SpanBitReader(buffer, bitOffset);

            // WHEN a byte is peeked
            var value = bitCount == 8
                ? bitReader.PeekByte()
                : bitReader.PeekByte(bitCount);

            // THEN the byte is as expected
            value.Should().Be(expectedByte);

            // THEN the bit position is unchanged
            bitReader.BitPosition.Should().Be(bitOffset);
        }
        else
        {
            // GIVEN a bit reader
            // WHEN a byte is peeked
            var action = () =>
            {
                var bitReader = new SpanBitReader(buffer, bitOffset);
                var value = bitCount == 8
                    ? bitReader.PeekByte()
                    : bitReader.PeekByte(bitCount);
            };

            // THEN an exception is thrown
            action.Should().Throw<EndOfStreamException>();
        }
    }

    #endregion
}
