using Chess.Structs;

namespace Chess.Test.Structs;

public class SquareTest
{
    [Fact]
    public void IsWithinBoard_WhenSquareWithinBoard_ReturnsTrue()
    {
        for (var x = 0; x < 8; x++)
            for (var y = 0; y < 8; y++)
                Assert.True(new Square(x, y).IsWithinBoard());
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(int.MaxValue, 2)]
    [InlineData(2, int.MaxValue)]
    [InlineData(int.MinValue, 1)]
    [InlineData(1, int.MinValue)]
    [InlineData(8, 0)]
    [InlineData(0, 8)]
    public void IsWithinBoard_WhenSquareOutsideBoard_ReturnsFalse(int x, int y)
    {
        var square = new Square(x, y);

        Assert.False(square.IsWithinBoard());
    }
}
