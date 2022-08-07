using System.Diagnostics.Eventing.Reader;

namespace Pico.Tests;

public class SpanBitWriterTests
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

        // WHEN the bit writer is created with a negative bit offset
        var action = () => { new SpanBitWriter(buffer, bitOffset); };

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

        // WHEN a bit writer is created
        var bitWriter = new SpanBitWriter(buffer, bitOffset);

        // THEN the bit writer properties are as expected
        bitWriter.BitLength.Should().Be(bufferLength * 8);
        bitWriter.BytePosition.Should().Be(expectedBytePosition);
        bitWriter.BitOffset.Should().Be(expectedBitOffset);
        bitWriter.BitPosition.Should().Be(bitOffset);
        bitWriter.RemainingBits.Should().Be(expectedRemainingBits);
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

        // WHEN the bit writer is created and set to an invalid bit position
        var action = () =>
        {
            var bitWriter = new SpanBitWriter(buffer);
            return bitWriter.BitPosition = invalidBitPosition;
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

        // GIVEN a bit writer
        var bitWriter = new SpanBitWriter(buffer);

        // WHEN the bit writer is set to a valid bit position
        bitWriter.BitPosition = bitOffset;

        // THEN the bit writer properties are as expected
        bitWriter.BitLength.Should().Be(bufferLength * 8);
        bitWriter.BytePosition.Should().Be(expectedBytePosition);
        bitWriter.BitOffset.Should().Be(expectedBitOffset);
        bitWriter.BitPosition.Should().Be(bitOffset);
        bitWriter.RemainingBits.Should().Be(expectedRemainingBits);
    }

    [Theory]
    [InlineData(4, -1)]
    [InlineData(4, 5)]
    public void SetBytePositionOutOfRange(
        int bufferLength, int invalidBytePosition)
    {
        // GIVEN a buffer
        var buffer = new byte[bufferLength];

        // WHEN the bit writer is created and set to an invalid byte position
        var action = () =>
        {
            var bitWriter = new SpanBitWriter(buffer);
            return bitWriter.BytePosition = invalidBytePosition;
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

        // GIVEN a bit writer
        var bitWriter = new SpanBitWriter(buffer, 1);

        // WHEN the bit writer is set to a valid bit position
        bitWriter.BytePosition = bytePosition;

        // THEN the bit writer properties are as expected
        bitWriter.BitLength.Should().Be(bufferLength * 8);
        bitWriter.BytePosition.Should().Be(expectedBytePosition);
        bitWriter.BitOffset.Should().Be(0);
        bitWriter.BitPosition.Should().Be(bytePosition * 8);
        bitWriter.RemainingBits.Should().Be(expectedRemainingBits);
    }

    #endregion

    #region [ Try Write/Poke Bit ]

    [Theory]
    [InlineData("", 0, false, false, "")]
    [InlineData("20", 0, false, true, "20")]
    [InlineData("20", 0, true, true, "A0")]
    [InlineData("20", 2, true, true, "20")]
    [InlineData("20", 2, false, true, "00")]
    [InlineData("20", 7, true, true, "21")]
    [InlineData("20", 8, true, false, "20")]
    [InlineData("2040", 0, false, true, "2040")]
    [InlineData("2040", 0, true, true, "A040")]
    [InlineData("2040", 2, true, true, "2040")]
    [InlineData("2040", 2, false, true, "0040")]
    [InlineData("2040", 7, true, true, "2140")]
    [InlineData("2040", 8, true, true, "20C0")]
    [InlineData("2040", 9, true, true, "2040")]
    [InlineData("2040", 9, false, true, "2000")]
    [InlineData("2040", 15, true, true, "2041")]
    [InlineData("2040", 16, true, false, "2040")]
    public void TryPokeBit(
        string hexBytes, int bitOffset, bool bit,
        bool expectedResult, string expectedBuffer)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit writer
        var bitWriter = new SpanBitWriter(buffer, bitOffset);

        // WHEN a bit is tried to be poked
        var result = bitWriter.TryPokeBit(bit);

        // THEN the result is as expected
        result.Should().Be(expectedResult);

        // THEN the buffer is as expected
        buffer.ToHexEncoding(true).Should().Be(expectedBuffer);

        // THEN the bit position is unchanged
        bitWriter.BitPosition.Should().Be(bitOffset);
    }

    [Theory]
    [InlineData("", 0, false, false, "")]
    [InlineData("20", 0, false, true, "20")]
    [InlineData("20", 0, true, true, "A0")]
    [InlineData("20", 2, true, true, "20")]
    [InlineData("20", 2, false, true, "00")]
    [InlineData("20", 7, true, true, "21")]
    [InlineData("20", 8, true, false, "20")]
    [InlineData("2040", 0, false, true, "2040")]
    [InlineData("2040", 0, true, true, "A040")]
    [InlineData("2040", 2, true, true, "2040")]
    [InlineData("2040", 2, false, true, "0040")]
    [InlineData("2040", 7, true, true, "2140")]
    [InlineData("2040", 8, true, true, "20C0")]
    [InlineData("2040", 9, true, true, "2040")]
    [InlineData("2040", 9, false, true, "2000")]
    [InlineData("2040", 15, true, true, "2041")]
    [InlineData("2040", 16, true, false, "2040")]
    public void PokeBit(
        string hexBytes, int bitOffset, bool bit,
        bool expectedResult, string expectedBuffer)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        if (expectedResult)
        {
            // GIVEN a bit writer
            var bitWriter = new SpanBitWriter(buffer, bitOffset);

            // WHEN a bit is poked
            bitWriter.PokeBit(bit);

            // THEN the buffer is as expected
            buffer.ToHexEncoding(true).Should().Be(expectedBuffer);

            // THEN the bit position is unchanged
            bitWriter.BitPosition.Should().Be(bitOffset);
        }
        else
        {
            // GIVEN a bit writer
            // WHEN a bit is poked
            var action = () =>
            {
                var bitWriter = new SpanBitWriter(buffer, bitOffset);
                bitWriter.PokeBit(bit);
            };

            // THEN an exception is thrown
            action.Should().Throw<EndOfStreamException>();
        }
    }

    [Theory]
    [InlineData("", 0, false, false, "")]
    [InlineData("20", 0, false, true, "20")]
    [InlineData("20", 0, true, true, "A0")]
    [InlineData("20", 2, true, true, "20")]
    [InlineData("20", 2, false, true, "00")]
    [InlineData("20", 7, true, true, "21")]
    [InlineData("20", 8, true, false, "20")]
    [InlineData("2040", 0, false, true, "2040")]
    [InlineData("2040", 0, true, true, "A040")]
    [InlineData("2040", 2, true, true, "2040")]
    [InlineData("2040", 2, false, true, "0040")]
    [InlineData("2040", 7, true, true, "2140")]
    [InlineData("2040", 8, true, true, "20C0")]
    [InlineData("2040", 9, true, true, "2040")]
    [InlineData("2040", 9, false, true, "2000")]
    [InlineData("2040", 15, true, true, "2041")]
    [InlineData("2040", 16, true, false, "2040")]
    public void TryWriteBit(
        string hexBytes, int bitOffset, bool bit,
        bool expectedResult, string expectedBuffer)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit writer
        var bitWriter = new SpanBitWriter(buffer, bitOffset);

        // WHEN a bit is tried to be written
        var result = bitWriter.TryWriteBit(bit);

        // THEN the result is as expected
        result.Should().Be(expectedResult);

        // THEN the buffer is as expected
        buffer.ToHexEncoding(true).Should().Be(expectedBuffer);

        if (expectedResult)
        {
            // THEN the bit position is increased
            bitWriter.BitPosition.Should().Be(bitOffset + 1);
        }
        else
        {
            // THEN the bit position is unchanged
            bitWriter.BitPosition.Should().Be(bitOffset);
        }
    }

    [Theory]
    [InlineData("", 0, false, false, "")]
    [InlineData("20", 0, false, true, "20")]
    [InlineData("20", 0, true, true, "A0")]
    [InlineData("20", 2, true, true, "20")]
    [InlineData("20", 2, false, true, "00")]
    [InlineData("20", 7, true, true, "21")]
    [InlineData("20", 8, true, false, "20")]
    [InlineData("2040", 0, false, true, "2040")]
    [InlineData("2040", 0, true, true, "A040")]
    [InlineData("2040", 2, true, true, "2040")]
    [InlineData("2040", 2, false, true, "0040")]
    [InlineData("2040", 7, true, true, "2140")]
    [InlineData("2040", 8, true, true, "20C0")]
    [InlineData("2040", 9, true, true, "2040")]
    [InlineData("2040", 9, false, true, "2000")]
    [InlineData("2040", 15, true, true, "2041")]
    [InlineData("2040", 16, true, false, "2040")]
    public void WriteBit(
        string hexBytes, int bitOffset, bool bit,
        bool expectedResult, string expectedBuffer)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        if (expectedResult)
        {
            // GIVEN a bit writer
            var bitWriter = new SpanBitWriter(buffer, bitOffset);

            // WHEN a bit is written
            bitWriter.WriteBit(bit);

            // THEN the buffer is as expected
            buffer.ToHexEncoding(true).Should().Be(expectedBuffer);
        }
        else
        {
            // GIVEN a bit writer
            // WHEN a bit is written
            var action = () =>
            {
                var bitWriter = new SpanBitWriter(buffer, bitOffset);
                bitWriter.WriteBit(bit);
            };

            // THEN an exception is thrown
            action.Should().Throw<EndOfStreamException>();
        }
    }

    #endregion

    #region [ Try Write/Poke Byte ]

    [Theory]
    [InlineData("20", 0, -1)]
    [InlineData("20", 0, 9)]
    public void TryPokeByteInvalidBitCount(
        string hexBytes, int bitOffset, int bitCount)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit writer
        // WHEN a byte is tried to be poked
        var action = () =>
        {
            var bitWriter = new SpanBitWriter(buffer, bitOffset);
            bitWriter.TryPokeByte(0, bitCount);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("21", 0, 0xBD, 4, true, "D1")]
    [InlineData("21", 0, 0xBD, 6, true, "F5")]
    [InlineData("21", 0, 0xBD, 8, true, "BD")]
    [InlineData("21", 4, 0xBD, 4, true, "2D")]
    [InlineData("21", 6, 0xBD, 4, false, "21")]
    [InlineData("2141", 0, 0xBD, 4, true, "D141")]
    [InlineData("2141", 0, 0xBD, 6, true, "F541")]
    [InlineData("2141", 0, 0xBD, 8, true, "BD41")]
    [InlineData("2141", 4, 0xBD, 6, true, "2F41")]
    [InlineData("2141", 6, 0xBD, 8, true, "22F5")]
    [InlineData("2141", 8, 0xBD, 8, true, "21BD")]
    [InlineData("2141", 8, 0xBD, 4, true, "21D1")]
    [InlineData("2141", 10, 0xBD, 4, true, "2175")]
    [InlineData("2141", 12, 0xBD, 8, false, "2141")]
    public void TryPokeByte(
        string hexBytes, int bitOffset, byte value, int bitCount,
        bool expectedResult, string expectedBuffer)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit writer
        var bitWriter = new SpanBitWriter(buffer, bitOffset);

        // WHEN a byte is tried to be poked
        var result = bitWriter.TryPokeByte(value, bitCount);

        // THEN the result is as expected
        result.Should().Be(expectedResult);

        // THEN the buffer is as expected
        buffer.ToHexEncoding(true).Should().Be(expectedBuffer);

        // THEN the bit position is unchanged
        bitWriter.BitPosition.Should().Be(bitOffset);
    }

    [Theory]
    [InlineData("20", 0, -1)]
    [InlineData("20", 0, 9)]
    public void PokeByteInvalidBitCount(
        string hexBytes, int bitOffset, int bitCount)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit writer
        // WHEN a byte is poked
        var action = () =>
        {
            var bitWriter = new SpanBitWriter(buffer, bitOffset);
            bitWriter.PokeByte(0, bitCount);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("21", 0, 0xBD, 4, true, "D1")]
    [InlineData("21", 0, 0xBD, 6, true, "F5")]
    [InlineData("21", 0, 0xBD, 8, true, "BD")]
    [InlineData("21", 4, 0xBD, 4, true, "2D")]
    [InlineData("21", 6, 0xBD, 4, false, "21")]
    [InlineData("2141", 0, 0xBD, 4, true, "D141")]
    [InlineData("2141", 0, 0xBD, 6, true, "F541")]
    [InlineData("2141", 0, 0xBD, 8, true, "BD41")]
    [InlineData("2141", 4, 0xBD, 6, true, "2F41")]
    [InlineData("2141", 6, 0xBD, 8, true, "22F5")]
    [InlineData("2141", 8, 0xBD, 8, true, "21BD")]
    [InlineData("2141", 8, 0xBD, 4, true, "21D1")]
    [InlineData("2141", 10, 0xBD, 4, true, "2175")]
    [InlineData("2141", 12, 0xBD, 8, false, "2141")]
    public void PokeByte(
        string hexBytes, int bitOffset, byte value, int bitCount,
        bool expectedResult, string expectedBuffer)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        if (expectedResult)
        {
            // GIVEN a bit writer
            var bitWriter = new SpanBitWriter(buffer, bitOffset);

            // WHEN a byte is poked
            bitWriter.PokeByte(value, bitCount);

            // THEN the buffer is as expected
            buffer.ToHexEncoding(true).Should().Be(expectedBuffer);

            // THEN the bit position is unchanged
            bitWriter.BitPosition.Should().Be(bitOffset);
        }
        else
        {
            // GIVEN a bit writer
            // WHEN a byte is poked
            var action = () =>
            {
                var bitWriter = new SpanBitWriter(buffer, bitOffset);
                bitWriter.PokeByte(value, bitCount);
            };

            // THEN an exception is thrown
            action.Should().Throw<EndOfStreamException>();
        }
    }

    [Theory]
    [InlineData("20", 0, -1)]
    [InlineData("20", 0, 9)]
    public void TryWriteByteInvalidBitCount(
        string hexBytes, int bitOffset, int bitCount)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit writer
        // WHEN a byte is tried to be written
        var action = () =>
        {
            var bitWriter = new SpanBitWriter(buffer, bitOffset);
            bitWriter.TryWriteByte(0, bitCount);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("21", 0, 0xBD, 4, true, "D1")]
    [InlineData("21", 0, 0xBD, 6, true, "F5")]
    [InlineData("21", 0, 0xBD, 8, true, "BD")]
    [InlineData("21", 4, 0xBD, 4, true, "2D")]
    [InlineData("21", 6, 0xBD, 4, false, "21")]
    [InlineData("2141", 0, 0xBD, 4, true, "D141")]
    [InlineData("2141", 0, 0xBD, 6, true, "F541")]
    [InlineData("2141", 0, 0xBD, 8, true, "BD41")]
    [InlineData("2141", 4, 0xBD, 6, true, "2F41")]
    [InlineData("2141", 6, 0xBD, 8, true, "22F5")]
    [InlineData("2141", 8, 0xBD, 8, true, "21BD")]
    [InlineData("2141", 8, 0xBD, 4, true, "21D1")]
    [InlineData("2141", 10, 0xBD, 4, true, "2175")]
    [InlineData("2141", 12, 0xBD, 8, false, "2141")]
    public void TryWriteByte(
        string hexBytes, int bitOffset, byte value, int bitCount,
        bool expectedResult, string expectedBuffer)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit writer
        var bitWriter = new SpanBitWriter(buffer, bitOffset);

        // WHEN a byte is tried to be written
        var result = bitWriter.TryWriteByte(value, bitCount);

        // THEN the result is as expected
        result.Should().Be(expectedResult);

        // THEN the buffer is as expected
        buffer.ToHexEncoding(true).Should().Be(expectedBuffer);

        if (expectedResult)
        {
            // THEN the bit position is increased
            bitWriter.BitPosition.Should().Be(bitOffset + bitCount);
        }
        else
        {
            // THEN the bit position is unchanged
            bitWriter.BitPosition.Should().Be(bitOffset);
        }
    }
    
    [Theory]
    [InlineData("20", 0, -1)]
    [InlineData("20", 0, 9)]
    public void WriteByteInvalidBitCount(
        string hexBytes, int bitOffset, int bitCount)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        // GIVEN a bit writer
        // WHEN a byte is written
        var action = () =>
        {
            var bitWriter = new SpanBitWriter(buffer, bitOffset);
            bitWriter.WriteByte(0, bitCount);
        };

        // THEN an exception is thrown
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("21", 0, 0xBD, 4, true, "D1")]
    [InlineData("21", 0, 0xBD, 6, true, "F5")]
    [InlineData("21", 0, 0xBD, 8, true, "BD")]
    [InlineData("21", 4, 0xBD, 4, true, "2D")]
    [InlineData("21", 6, 0xBD, 4, false, "21")]
    [InlineData("2141", 0, 0xBD, 4, true, "D141")]
    [InlineData("2141", 0, 0xBD, 6, true, "F541")]
    [InlineData("2141", 0, 0xBD, 8, true, "BD41")]
    [InlineData("2141", 4, 0xBD, 6, true, "2F41")]
    [InlineData("2141", 6, 0xBD, 8, true, "22F5")]
    [InlineData("2141", 8, 0xBD, 8, true, "21BD")]
    [InlineData("2141", 8, 0xBD, 4, true, "21D1")]
    [InlineData("2141", 10, 0xBD, 4, true, "2175")]
    [InlineData("2141", 12, 0xBD, 8, false, "2141")]
    public void WriteByte(
        string hexBytes, int bitOffset, byte value, int bitCount,
        bool expectedResult, string expectedBuffer)
    {
        // GIVEN a buffer
        var buffer = hexBytes.FromHexEncoding();

        if (expectedResult)
        {
            // GIVEN a bit writer
            var bitWriter = new SpanBitWriter(buffer, bitOffset);

            // WHEN a byte is written
            bitWriter.WriteByte(value, bitCount);

            // THEN the buffer is as expected
            buffer.ToHexEncoding(true).Should().Be(expectedBuffer);

            // THEN the bit position is increased
            bitWriter.BitPosition.Should().Be(bitOffset + bitCount);
        }
        else
        {
            // GIVEN a bit writer
            // WHEN a byte is written
            var action = () =>
            {
                var bitWriter = new SpanBitWriter(buffer, bitOffset);
                bitWriter.WriteByte(value, bitCount);
            };

            // THEN an exception is thrown
            action.Should().Throw<EndOfStreamException>();
        }
    }

    #endregion
}
