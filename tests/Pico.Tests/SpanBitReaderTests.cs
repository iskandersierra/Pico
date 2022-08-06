namespace Pico.Tests;

public class SpanBitReaderTests
{
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
}
