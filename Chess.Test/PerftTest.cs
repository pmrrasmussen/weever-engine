using Chess.Builders;

namespace Chess.Test;

public class PerftTest
{
    private Dictionary<int, long> depthToCount = new ()
    {
        { 1, 20 },
        { 2, 400 },
        { 3, 8902 },
        { 4, 197281 },
        { 5, 4865609 },
        { 6, 119060324 },
        { 7, 3195901860 },
        { 8, 84998978956 },
        { 9, 2439530234167 },
    };

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)] // this test is slooooow
    [InlineData(5)] // this test is slooooow
    [InlineData(6)] // this test is slooooow
    [InlineData(7)] // this test is slooooow
    [InlineData(8)] // this test is slooooow
    [InlineData(9)] // this test is slooooow
    public void Check(int depth)
    {
        var board = BoardBuilder.GetDefaultStartingPosition();
        long count = GetPossibleMovesCount(board, depth);
        long expectedCount = 0;
        for (int i = 1; i <= depth; i++)
        {
            expectedCount += depthToCount[i];
        }
        Assert.Equal(expectedCount, count);
    }

    private long GetPossibleMovesCount(Board board, int toDepth)
    {
        if (toDepth == 0)
            return 1;

        var moves = board.GetPseudoLegalMoves();
        long sum = 1;
        foreach(var move in moves)
        {
            board.MakeMove(move);
            sum += GetPossibleMovesCount(board, toDepth - 1);
            board.UndoLastMove();
        }

        return sum;
    }
}
