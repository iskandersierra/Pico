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
        var action = () => { new SpanBitReader(buffer, bitOffset); };

        // THEN an argument exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(4, 0, 0, 0, 32)]
    [InlineData(4, 1, 0, 1, 31)]
    [InlineData(4, 7, 0, 7, 25)]
    [InlineData(4, 8, 1, 0, 24)]
    [InlineData(4, 9, 1, 1, 23)]
    [InlineData(4, 15, 1, 7, 17)]
    [InlineData(4, 16, 2, 0, 16)]
    [InlineData(4, 17, 2, 1, 15)]
    [InlineData(4, 23, 2, 7, 9)]
    [InlineData(4, 24, 3, 0, 8)]
    [InlineData(4, 25, 3, 1, 7)]
    [InlineData(4, 31, 3, 7, 1)]
    [InlineData(4, 32, 4, 0, 0)]
    public void Create(
        int bufferLength, int bitOffset,
        int expectedBytePosition, int expectedBitOffset,
        int expectedRemainingBits)
    {
        // GIVEN a buffer
        var buffer = new byte[bufferLength];

        // WHEN a bit reader is created
        var bitReader = new SpanBitReader(buffer, bitOffset);

        // THEN the bit reader properties are as expected
        bitReader.BitLength.Should().Be(bufferLength * 8);
        bitReader.BytePosition.Should().Be(expectedBytePosition);
        bitReader.BitOffset.Should().Be(expectedBitOffset);
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
    [InlineData(4, 0, 0, 0, 32)]
    [InlineData(4, 1, 0, 1, 31)]
    [InlineData(4, 7, 0, 7, 25)]
    [InlineData(4, 8, 1, 0, 24)]
    [InlineData(4, 9, 1, 1, 23)]
    [InlineData(4, 15, 1, 7, 17)]
    [InlineData(4, 16, 2, 0, 16)]
    [InlineData(4, 17, 2, 1, 15)]
    [InlineData(4, 23, 2, 7, 9)]
    [InlineData(4, 24, 3, 0, 8)]
    [InlineData(4, 25, 3, 1, 7)]
    [InlineData(4, 31, 3, 7, 1)]
    [InlineData(4, 32, 4, 0, 0)]
    public void SetBitPosition(
        int bufferLength, int bitOffset,
        int expectedBytePosition, int expectedBitOffset,
        int expectedRemainingBits)
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
        bitReader.BitOffset.Should().Be(expectedBitOffset);
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
        var bitReader = new SpanBitReader(buffer, 1);

        // WHEN the bit reader is set to a valid bit position
        bitReader.BytePosition = bytePosition;

        // THEN the bit reader properties are as expected
        bitReader.BitLength.Should().Be(bufferLength * 8);
        bitReader.BytePosition.Should().Be(expectedBytePosition);
        bitReader.BitOffset.Should().Be(0);
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
        bool expectedResult, bool expectedValue)
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
        bit.Should().Be(expectedValue);

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
        bool expectedResult, bool expectedValue)
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
            bit.Should().Be(expectedValue);

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
        bool expectedResult, bool expectedValue)
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
        bit.Should().Be(expectedValue);

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
        bool expectedResult, bool expectedValue)
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
            bit.Should().Be(expectedValue);

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
            bitReader.TryPeekByte(out _, bitCount);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 5, 2, true, 0x03)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 5, 2, true, 0x03)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 12, 3, true, 0x05)]
    [InlineData("A76B", 13, 4, false, 0)]
    public void TryPeekByte(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, byte expectedValue)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        var bitReader = new SpanBitReader(buffer, bitOffset);

        // WHEN a byte is tried to be peek
        var result = bitCount == 8
            ? bitReader.TryPeekByte(out var value)
            : bitReader.TryPeekByte(out value, bitCount);

        // THEN the result is as expected
        result.Should().Be(expectedResult);

        // THEN the byte is as expected
        value.Should().Be(expectedValue);

        // THEN the bit position is unchanged
        bitReader.BitPosition.Should().Be(bitOffset);
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 5, 2, true, 0x03)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 5, 2, true, 0x03)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 12, 3, true, 0x05)]
    [InlineData("A76B", 13, 4, false, 0)]
    public void PeekByte(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, byte expectedValue)
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
            value.Should().Be(expectedValue);

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

    [Theory]
    [InlineData("20", 0, -1)]
    [InlineData("20", 0, 9)]
    public void TryReadByteInvalidBitCount(
        string hexBytes, int bitOffset, int bitCount)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        // WHEN a byte is tried to be read with invalid bit count
        var action = () =>
        {
            var bitReader = new SpanBitReader(buffer, bitOffset);
            bitReader.TryReadByte(out _, bitCount);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 5, 2, true, 0x03)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 5, 2, true, 0x03)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 12, 3, true, 0x05)]
    [InlineData("A76B", 13, 4, false, 0)]
    public void TryReadByte(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, byte expectedValue)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        var bitReader = new SpanBitReader(buffer, bitOffset);

        // WHEN a byte is tried to be read
        var result = bitCount == 8
            ? bitReader.TryReadByte(out var value)
            : bitReader.TryReadByte(out value, bitCount);

        // THEN the result is as expected
        result.Should().Be(expectedResult);

        // THEN the byte is as expected
        value.Should().Be(expectedValue);

        if (expectedResult)
        {
            // THEN the bit position is increased
            bitReader.BitPosition.Should().Be(bitOffset + bitCount);
        }
        else
        {
            // THEN the bit position is unchanged
            bitReader.BitPosition.Should().Be(bitOffset);
        }
    }

    [Theory]
    [InlineData("20", 0, -1)]
    [InlineData("20", 0, 9)]
    public void ReadByteInvalidBitCount(
        string hexBytes, int bitOffset, int bitCount)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        // WHEN a byte is read with invalid bit count
        var action = () =>
        {
            var bitReader = new SpanBitReader(buffer, bitOffset);
            bitReader.ReadByte(bitCount);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 5, 2, true, 0x03)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 5, 2, true, 0x03)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 12, 3, true, 0x05)]
    [InlineData("A76B", 13, 4, false, 0)]
    public void ReadByte(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, byte expectedValue)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        if (expectedResult)
        {
            // GIVEN a bit reader
            var bitReader = new SpanBitReader(buffer, bitOffset);

            // WHEN a byte is read
            var value = bitCount == 8
                ? bitReader.ReadByte()
                : bitReader.ReadByte(bitCount);

            // THEN the byte is as expected
            value.Should().Be(expectedValue);

            // THEN the bit position is increased
            bitReader.BitPosition.Should().Be(bitOffset + bitCount);
        }
        else
        {
            // GIVEN a bit reader
            // WHEN a byte is read
            var action = () =>
            {
                var bitReader = new SpanBitReader(buffer, bitOffset);
                var _ = bitCount == 8
                    ? bitReader.ReadByte()
                    : bitReader.ReadByte(bitCount);
            };

            // THEN an exception is thrown
            action.Should().Throw<EndOfStreamException>();
        }
    }

    #endregion

    #region [ Try Read/Peek Short ]

    [Theory]
    [InlineData("2040", 0, -1)]
    [InlineData("2040", 0, 17)]
    public void TryPeekShortInvalidBitCount(
        string hexBytes, int bitOffset, int bitCount)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        // WHEN a short is tried to be peek with invalid bit count
        var action = () =>
        {
            var bitReader = new SpanBitReader(buffer, bitOffset);
            bitReader.TryPeekShort(out _, bitCount);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 5, 2, true, 0x03)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 0, 16, true, 0xA76B)]
    [InlineData("A76B", 0, 14, true, 0x29DA)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 5, 2, true, 0x03)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 5, 10, true, 0x03B5)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 12, 3, true, 0x05)]
    [InlineData("A76B", 13, 4, false, 0)]
    public void TryPeekShort(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, ushort expectedValue)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        var bitReader = new SpanBitReader(buffer, bitOffset);

        // WHEN a short is tried to be peek
        var result = bitCount == 16
            ? bitReader.TryPeekShort(out var value)
            : bitReader.TryPeekShort(out value, bitCount);

        // THEN the result is as expected
        result.Should().Be(expectedResult);

        // THEN the short is as expected
        value.Should().Be(expectedValue);

        // THEN the bit position is unchanged
        bitReader.BitPosition.Should().Be(bitOffset);
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 5, 2, true, 0x03)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 0, 16, true, 0xA76B)]
    [InlineData("A76B", 0, 14, true, 0x29DA)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 5, 2, true, 0x03)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 5, 10, true, 0x03B5)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 12, 3, true, 0x05)]
    [InlineData("A76B", 13, 4, false, 0)]
    public void PeekShort(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, ushort expectedValue)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        if (expectedResult)
        {
            // GIVEN a bit reader
            var bitReader = new SpanBitReader(buffer, bitOffset);

            // WHEN a short is peeked
            var value = bitCount == 16
                ? bitReader.PeekShort()
                : bitReader.PeekShort(bitCount);

            // THEN the short is as expected
            value.Should().Be(expectedValue);

            // THEN the bit position is unchanged
            bitReader.BitPosition.Should().Be(bitOffset);
        }
        else
        {
            // GIVEN a bit reader
            // WHEN a short is peeked
            var action = () =>
            {
                var bitReader = new SpanBitReader(buffer, bitOffset);
                var _ = bitCount == 16
                    ? bitReader.PeekShort()
                    : bitReader.PeekShort(bitCount);
            };

            // THEN an exception is thrown
            action.Should().Throw<EndOfStreamException>();
        }
    }

    [Theory]
    [InlineData("2040", 0, -1)]
    [InlineData("2040", 0, 17)]
    public void TryReadShortInvalidBitCount(
        string hexBytes, int bitOffset, int bitCount)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        // WHEN a short is tried to be read with an invalid bit count
        var action = () =>
        {
            var bitReader = new SpanBitReader(buffer, bitOffset);
            bitReader.TryReadShort(out _, bitCount);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 5, 2, true, 0x03)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 0, 16, true, 0xA76B)]
    [InlineData("A76B", 0, 14, true, 0x29DA)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 5, 2, true, 0x03)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 5, 10, true, 0x03B5)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 12, 3, true, 0x05)]
    [InlineData("A76B", 13, 4, false, 0)]
    public void TryReadShort(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, ushort expectedValue)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        var bitReader = new SpanBitReader(buffer, bitOffset);

        // WHEN a short is tried to be read
        var result = bitCount == 16
            ? bitReader.TryReadShort(out var value)
            : bitReader.TryReadShort(out value, bitCount);

        // THEN the result is as expected
        result.Should().Be(expectedResult);

        // THEN the short is as expected
        value.Should().Be(expectedValue);

        if (expectedResult)
        {
            // THEN the bit position is increased
            bitReader.BitPosition.Should().Be(bitOffset + bitCount);
        }
        else
        {
            // THEN the bit position is unchanged
            bitReader.BitPosition.Should().Be(bitOffset);
        }
    }

    [Theory]
    [InlineData("2040", 0, -1)]
    [InlineData("2040", 0, 17)]
    public void ReadShortInvalidBitCount(
        string hexBytes, int bitOffset, int bitCount)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        // WHEN a short is read with invalid bit count
        var action = () =>
        {
            var bitReader = new SpanBitReader(buffer, bitOffset);
            bitReader.ReadShort(bitCount);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 5, 2, true, 0x03)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 0, 16, true, 0xA76B)]
    [InlineData("A76B", 0, 14, true, 0x29DA)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 5, 2, true, 0x03)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 5, 10, true, 0x03B5)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 12, 3, true, 0x05)]
    [InlineData("A76B", 13, 4, false, 0)]
    public void ReadShort(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, ushort expectedValue)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        if (expectedResult)
        {
            // GIVEN a bit reader
            var bitReader = new SpanBitReader(buffer, bitOffset);

            // WHEN a short is read
            var value = bitCount == 16
                ? bitReader.ReadShort()
                : bitReader.ReadShort(bitCount);

            // THEN the short is as expected
            value.Should().Be(expectedValue);

            // THEN the bit position is increased
            bitReader.BitPosition.Should().Be(bitOffset + bitCount);
        }
        else
        {
            // GIVEN a bit reader
            // WHEN a short is read
            var action = () =>
            {
                var bitReader = new SpanBitReader(buffer, bitOffset);
                var _ = bitCount == 16
                    ? bitReader.ReadShort()
                    : bitReader.ReadShort(bitCount);
            };

            // THEN an exception is thrown
            action.Should().Throw<EndOfStreamException>();
        }
    }

    #endregion

    #region [ Try Read/Peek Int ]

    [Theory]
    [InlineData("10204080", 0, -1)]
    [InlineData("10204080", 0, 33)]
    public void TryPeekIntInvalidBitCount(
        string hexBytes, int bitOffset, int bitCount)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        // WHEN a int is tried to be peek with invalid bit count
        var action = () =>
        {
            var bitReader = new SpanBitReader(buffer, bitOffset);
            bitReader.TryPeekInt(out _, bitCount);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 5, 2, true, 0x03)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 0, 16, true, 0xA76B)]
    [InlineData("A76B", 0, 14, true, 0x29DA)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 5, 2, true, 0x03)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 5, 10, true, 0x03B5)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 12, 3, true, 0x05)]
    [InlineData("A76B", 13, 4, false, 0)]
    [InlineData("A76BC27A", 0, 32, true, 0xA76BC27A)]
    [InlineData("A76BC27A", 0, 24, true, 0xA76BC2)]
    [InlineData("A76BC27A", 8, 24, true, 0x6BC27A)]
    [InlineData("A76BC27A", 16, 16, true, 0xC27A)]
    [InlineData("A76BC27A", 24, 8, true, 0x7A)]
    [InlineData("A76BC27A", 3, 24, true, 0x3B5E13)]
    public void TryPeekInt(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, uint expectedValue)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        var bitReader = new SpanBitReader(buffer, bitOffset);

        // WHEN a int is tried to be peek
        var result = bitCount == 32
            ? bitReader.TryPeekInt(out var value)
            : bitReader.TryPeekInt(out value, bitCount);

        // THEN the result is as expected
        result.Should().Be(expectedResult);

        // THEN the int is as expected
        value.Should().Be(expectedValue);

        // THEN the bit position is not changed
        bitReader.BitPosition.Should().Be(bitOffset);
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 5, 2, true, 0x03)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 0, 16, true, 0xA76B)]
    [InlineData("A76B", 0, 14, true, 0x29DA)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 5, 2, true, 0x03)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 5, 10, true, 0x03B5)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 12, 3, true, 0x05)]
    [InlineData("A76B", 13, 4, false, 0)]
    [InlineData("A76BC27A", 0, 32, true, 0xA76BC27A)]
    [InlineData("A76BC27A", 0, 24, true, 0xA76BC2)]
    [InlineData("A76BC27A", 8, 24, true, 0x6BC27A)]
    [InlineData("A76BC27A", 16, 16, true, 0xC27A)]
    [InlineData("A76BC27A", 24, 8, true, 0x7A)]
    [InlineData("A76BC27A", 3, 24, true, 0x3B5E13)]
    public void PeekInt(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, uint expectedValue)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        if (expectedResult)
        {
            // GIVEN a bit reader
            var bitReader = new SpanBitReader(buffer, bitOffset);

            // WHEN a int is peeked
            var value = bitCount == 32
                ? bitReader.PeekInt()
                : bitReader.PeekInt(bitCount);

            // THEN the int is as expected
            value.Should().Be(expectedValue);

            // THEN the bit position is not changed
            bitReader.BitPosition.Should().Be(bitOffset);
        }
        else
        {
            // GIVEN a bit reader
            // WHEN a int is peeked
            var action = () =>
            {
                var bitReader = new SpanBitReader(buffer, bitOffset);
                var _ = bitCount == 32
                    ? bitReader.PeekInt()
                    : bitReader.PeekInt(bitCount);
            };

            // THEN an exception is thrown
            action.Should().Throw<EndOfStreamException>();
        }
    }

    [Theory]
    [InlineData("10204080", 0, -1)]
    [InlineData("10204080", 0, 33)]
    public void TryReadIntInvalidBitCount(
        string hexBytes, int bitOffset, int bitCount)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        // WHEN a int is tried to be read with an invalid bit count
        var action = () =>
        {
            var bitReader = new SpanBitReader(buffer, bitOffset);
            bitReader.TryReadInt(out _, bitCount);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 5, 2, true, 0x03)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 0, 16, true, 0xA76B)]
    [InlineData("A76B", 0, 14, true, 0x29DA)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 5, 2, true, 0x03)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 5, 10, true, 0x03B5)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 12, 3, true, 0x05)]
    [InlineData("A76B", 13, 4, false, 0)]
    [InlineData("A76BC27A", 0, 32, true, 0xA76BC27A)]
    [InlineData("A76BC27A", 0, 24, true, 0xA76BC2)]
    [InlineData("A76BC27A", 8, 24, true, 0x6BC27A)]
    [InlineData("A76BC27A", 16, 16, true, 0xC27A)]
    [InlineData("A76BC27A", 24, 8, true, 0x7A)]
    [InlineData("A76BC27A", 3, 24, true, 0x3B5E13)]
    public void TryReadInt(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, uint expectedValue)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        var bitReader = new SpanBitReader(buffer, bitOffset);

        // WHEN a int is tried to be read
        var result = bitCount == 32
            ? bitReader.TryReadInt(out var value)
            : bitReader.TryReadInt(out value, bitCount);

        // THEN the result is as expected
        result.Should().Be(expectedResult);

        // THEN the int is as expected
        value.Should().Be(expectedValue);

        if (expectedResult)
        {
            // THEN the bit position is increased
            bitReader.BitPosition.Should().Be(bitOffset + bitCount);
        }
        else
        {
            // THEN the bit position is unchanged
            bitReader.BitPosition.Should().Be(bitOffset);
        }
    }

    [Theory]
    [InlineData("10204080", 0, -1)]
    [InlineData("10204080", 0, 33)]
    public void ReadIntInvalidBitCount(
        string hexBytes, int bitOffset, int bitCount)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        // WHEN a int is read with an invalid bit count
        var action = () =>
        {
            var bitReader = new SpanBitReader(buffer, bitOffset);
            bitReader.ReadInt(bitCount);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 5, 2, true, 0x03)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 0, 16, true, 0xA76B)]
    [InlineData("A76B", 0, 14, true, 0x29DA)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 5, 2, true, 0x03)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 5, 10, true, 0x03B5)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 12, 3, true, 0x05)]
    [InlineData("A76B", 13, 4, false, 0)]
    [InlineData("A76BC27A", 0, 32, true, 0xA76BC27A)]
    [InlineData("A76BC27A", 0, 24, true, 0xA76BC2)]
    [InlineData("A76BC27A", 8, 24, true, 0x6BC27A)]
    [InlineData("A76BC27A", 16, 16, true, 0xC27A)]
    [InlineData("A76BC27A", 24, 8, true, 0x7A)]
    [InlineData("A76BC27A", 3, 24, true, 0x3B5E13)]
    public void ReadInt(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, uint expectedValue)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        if (expectedResult)
        {
            // GIVEN a bit reader
            var bitReader = new SpanBitReader(buffer, bitOffset);

            // WHEN a int is read
            var value = bitCount == 32
                ? bitReader.ReadInt()
                : bitReader.ReadInt(bitCount);

            // THEN the int is as expected
            value.Should().Be(expectedValue);

            // THEN the bit position is increased
            bitReader.BitPosition.Should().Be(bitOffset + bitCount);
        }
        else
        {
            // GIVEN a bit reader
            // WHEN a int is read
            var action = () =>
            {
                var bitReader = new SpanBitReader(buffer, bitOffset);
                var _ = bitCount == 32
                    ? bitReader.ReadInt()
                    : bitReader.ReadInt(bitCount);
            };

            // THEN an exception is thrown
            action.Should().Throw<EndOfStreamException>();
        }
    }

    #endregion

    #region [ Try Read/Peek Long ]

    [Theory]
    [InlineData("1020408010204080", 0, -1)]
    [InlineData("1020408010204080", 0, 65)]
    public void TryPeekLongInvalidBitCount(
        string hexBytes, int bitOffset, int bitCount)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        // WHEN a long is tried to be peek with invalid bit count
        var action = () =>
        {
            var bitReader = new SpanBitReader(buffer, bitOffset);
            bitReader.TryPeekLong(out _, bitCount);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 5, 2, true, 0x03)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 0, 16, true, 0xA76B)]
    [InlineData("A76B", 0, 14, true, 0x29DA)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 5, 2, true, 0x03)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 5, 10, true, 0x03B5)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 12, 3, true, 0x05)]
    [InlineData("A76B", 13, 4, false, 0)]
    [InlineData("A76BC27A", 0, 32, true, 0xA76BC27A)]
    [InlineData("A76BC27A", 0, 24, true, 0xA76BC2)]
    [InlineData("A76BC27A", 8, 24, true, 0x6BC27A)]
    [InlineData("A76BC27A", 16, 16, true, 0xC27A)]
    [InlineData("A76BC27A", 24, 8, true, 0x7A)]
    [InlineData("A76BC27A", 3, 24, true, 0x3B5E13)]
    [InlineData("A76BC27A3D1D6A6E", 0, 64, true, 0xA76BC27A3D1D6A6E)]
    [InlineData("A76BC27A3D1D6A6E", 0, 56, true, 0xA76BC27A3D1D6A)]
    [InlineData("A76BC27A3D1D6A6E", 0, 48, true, 0xA76BC27A3D1D)]
    [InlineData("A76BC27A3D1D6A6E", 0, 40, true, 0xA76BC27A3D)]
    [InlineData("A76BC27A3D1D6A6E", 0, 32, true, 0xA76BC27A)]
    [InlineData("A76BC27A3D1D6A6E", 8, 48, true, 0x6BC27A3D1D6A)]
    [InlineData("A76BC27A3D1D6A6E", 16, 48, true, 0xC27A3D1D6A6E)]
    [InlineData("A76BC27A3D1D6A6E", 13, 36, true, 0x0784F47A3A)]
    public void TryPeekLong(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, ulong expectedValue)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        var bitReader = new SpanBitReader(buffer, bitOffset);

        // WHEN a long is tried to be peek
        var result = bitCount == 64
            ? bitReader.TryPeekLong(out var value)
            : bitReader.TryPeekLong(out value, bitCount);

        // THEN the result is as expected
        result.Should().Be(expectedResult);

        // THEN the long is as expected
        value.Should().Be(expectedValue);

        // THEN the bit position is not changed
        bitReader.BitPosition.Should().Be(bitOffset);
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 5, 2, true, 0x03)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 0, 16, true, 0xA76B)]
    [InlineData("A76B", 0, 14, true, 0x29DA)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 5, 2, true, 0x03)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 5, 10, true, 0x03B5)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 12, 3, true, 0x05)]
    [InlineData("A76B", 13, 4, false, 0)]
    [InlineData("A76BC27A", 0, 32, true, 0xA76BC27A)]
    [InlineData("A76BC27A", 0, 24, true, 0xA76BC2)]
    [InlineData("A76BC27A", 8, 24, true, 0x6BC27A)]
    [InlineData("A76BC27A", 16, 16, true, 0xC27A)]
    [InlineData("A76BC27A", 24, 8, true, 0x7A)]
    [InlineData("A76BC27A", 3, 24, true, 0x3B5E13)]
    [InlineData("A76BC27A3D1D6A6E", 0, 64, true, 0xA76BC27A3D1D6A6E)]
    [InlineData("A76BC27A3D1D6A6E", 0, 56, true, 0xA76BC27A3D1D6A)]
    [InlineData("A76BC27A3D1D6A6E", 0, 48, true, 0xA76BC27A3D1D)]
    [InlineData("A76BC27A3D1D6A6E", 0, 40, true, 0xA76BC27A3D)]
    [InlineData("A76BC27A3D1D6A6E", 0, 32, true, 0xA76BC27A)]
    [InlineData("A76BC27A3D1D6A6E", 8, 48, true, 0x6BC27A3D1D6A)]
    [InlineData("A76BC27A3D1D6A6E", 16, 48, true, 0xC27A3D1D6A6E)]
    [InlineData("A76BC27A3D1D6A6E", 13, 36, true, 0x0784F47A3A)]
    public void PeekLong(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, ulong expectedValue)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        if (expectedResult)
        {
            // GIVEN a bit reader
            var bitReader = new SpanBitReader(buffer, bitOffset);

            // WHEN a long is peeked
            var value = bitCount == 64
                ? bitReader.PeekLong()
                : bitReader.PeekLong(bitCount);

            // THEN the long is as expected
            value.Should().Be(expectedValue);

            // THEN the bit position is not changed
            bitReader.BitPosition.Should().Be(bitOffset);
        }
        else
        {
            // GIVEN a bit reader
            // WHEN a long is peeked
            var action = () =>
            {
                var bitReader = new SpanBitReader(buffer, bitOffset);
                var _ = bitCount == 64
                    ? bitReader.PeekLong()
                    : bitReader.PeekLong(bitCount);
            };

            // THEN an exception is thrown
            action.Should().Throw<EndOfStreamException>();
        }
    }

    [Theory]
    [InlineData("1020408010204080", 0, -1)]
    [InlineData("1020408010204080", 0, 65)]
    public void TryReadLongInvalidBitCount(
        string hexBytes, int bitOffset, int bitCount)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        // WHEN a long is tried to be read with an invalid bit count
        var action = () =>
        {
            var bitReader = new SpanBitReader(buffer, bitOffset);
            bitReader.TryReadLong(out _, bitCount);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 5, 2, true, 0x03)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 0, 16, true, 0xA76B)]
    [InlineData("A76B", 0, 14, true, 0x29DA)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 5, 2, true, 0x03)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 5, 10, true, 0x03B5)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 12, 3, true, 0x05)]
    [InlineData("A76B", 13, 4, false, 0)]
    [InlineData("A76BC27A", 0, 32, true, 0xA76BC27A)]
    [InlineData("A76BC27A", 0, 24, true, 0xA76BC2)]
    [InlineData("A76BC27A", 8, 24, true, 0x6BC27A)]
    [InlineData("A76BC27A", 16, 16, true, 0xC27A)]
    [InlineData("A76BC27A", 24, 8, true, 0x7A)]
    [InlineData("A76BC27A", 3, 24, true, 0x3B5E13)]
    [InlineData("A76BC27A3D1D6A6E", 0, 64, true, 0xA76BC27A3D1D6A6E)]
    [InlineData("A76BC27A3D1D6A6E", 0, 56, true, 0xA76BC27A3D1D6A)]
    [InlineData("A76BC27A3D1D6A6E", 0, 48, true, 0xA76BC27A3D1D)]
    [InlineData("A76BC27A3D1D6A6E", 0, 40, true, 0xA76BC27A3D)]
    [InlineData("A76BC27A3D1D6A6E", 0, 32, true, 0xA76BC27A)]
    [InlineData("A76BC27A3D1D6A6E", 8, 48, true, 0x6BC27A3D1D6A)]
    [InlineData("A76BC27A3D1D6A6E", 16, 48, true, 0xC27A3D1D6A6E)]
    [InlineData("A76BC27A3D1D6A6E", 13, 36, true, 0x0784F47A3A)]
    public void TryReadLong(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, ulong expectedValue)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        var bitReader = new SpanBitReader(buffer, bitOffset);

        // WHEN a long is tried to be read
        var result = bitCount == 64
            ? bitReader.TryReadLong(out var value)
            : bitReader.TryReadLong(out value, bitCount);

        // THEN the result is as expected
        result.Should().Be(expectedResult);

        // THEN the long is as expected
        value.Should().Be(expectedValue);

        if (expectedResult)
        {
            // THEN the bit position is increased
            bitReader.BitPosition.Should().Be(bitOffset + bitCount);
        }
        else
        {
            // THEN the bit position is unchanged
            bitReader.BitPosition.Should().Be(bitOffset);
        }
    }

    [Theory]
    [InlineData("1020408010204080", 0, -1)]
    [InlineData("1020408010204080", 0, 65)]
    public void ReadLongInvalidBitCount(
        string hexBytes, int bitOffset, int bitCount)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit reader
        // WHEN a long is read with an invalid bit count
        var action = () =>
        {
            var bitReader = new SpanBitReader(buffer, bitOffset);
            bitReader.ReadLong(bitCount);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("", 0, 1, false, 0)]
    [InlineData("A7", 0, 8, true, 0xA7)]
    [InlineData("A7", 1, 8, false, 0)]
    [InlineData("A7", 2, 6, true, 0x27)]
    [InlineData("A7", 5, 2, true, 0x03)]
    [InlineData("A76B", 0, 8, true, 0xA7)]
    [InlineData("A76B", 0, 16, true, 0xA76B)]
    [InlineData("A76B", 0, 14, true, 0x29DA)]
    [InlineData("A76B", 2, 6, true, 0x27)]
    [InlineData("A76B", 5, 2, true, 0x03)]
    [InlineData("A76B", 1, 8, true, 0x4E)]
    [InlineData("A76B", 5, 8, true, 0xED)]
    [InlineData("A76B", 5, 10, true, 0x03B5)]
    [InlineData("A76B", 8, 8, true, 0x6B)]
    [InlineData("A76B", 9, 8, false, 0)]
    [InlineData("A76B", 9, 7, true, 0x6B)]
    [InlineData("A76B", 12, 3, true, 0x05)]
    [InlineData("A76B", 13, 4, false, 0)]
    [InlineData("A76BC27A", 0, 32, true, 0xA76BC27A)]
    [InlineData("A76BC27A", 0, 24, true, 0xA76BC2)]
    [InlineData("A76BC27A", 8, 24, true, 0x6BC27A)]
    [InlineData("A76BC27A", 16, 16, true, 0xC27A)]
    [InlineData("A76BC27A", 24, 8, true, 0x7A)]
    [InlineData("A76BC27A", 3, 24, true, 0x3B5E13)]
    [InlineData("A76BC27A3D1D6A6E", 0, 64, true, 0xA76BC27A3D1D6A6E)]
    [InlineData("A76BC27A3D1D6A6E", 0, 56, true, 0xA76BC27A3D1D6A)]
    [InlineData("A76BC27A3D1D6A6E", 0, 48, true, 0xA76BC27A3D1D)]
    [InlineData("A76BC27A3D1D6A6E", 0, 40, true, 0xA76BC27A3D)]
    [InlineData("A76BC27A3D1D6A6E", 0, 32, true, 0xA76BC27A)]
    [InlineData("A76BC27A3D1D6A6E", 8, 48, true, 0x6BC27A3D1D6A)]
    [InlineData("A76BC27A3D1D6A6E", 16, 48, true, 0xC27A3D1D6A6E)]
    [InlineData("A76BC27A3D1D6A6E", 13, 36, true, 0x0784F47A3A)]
    public void ReadLong(
        string hexBytes, int bitOffset, int bitCount,
        bool expectedResult, ulong expectedValue)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        if (expectedResult)
        {
            // GIVEN a bit reader
            var bitReader = new SpanBitReader(buffer, bitOffset);

            // WHEN a long is read
            var value = bitCount == 64
                ? bitReader.ReadLong()
                : bitReader.ReadLong(bitCount);

            // THEN the long is as expected
            value.Should().Be(expectedValue);

            // THEN the bit position is increased
            bitReader.BitPosition.Should().Be(bitOffset + bitCount);
        }
        else
        {
            // GIVEN a bit reader
            // WHEN a long is read
            var action = () =>
            {
                var bitReader = new SpanBitReader(buffer, bitOffset);
                var _ = bitCount == 64
                    ? bitReader.ReadLong()
                    : bitReader.ReadLong(bitCount);
            };

            // THEN an exception is thrown
            action.Should().Throw<EndOfStreamException>();
        }
    }

    #endregion
}
