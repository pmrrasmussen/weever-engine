using Chess.Builders;
using Xunit.Abstractions;

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

    private readonly ITestOutputHelper _testOutputHelper;

    public PerftTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 1, 48, 8)]
    [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 2, 2039, 351)]
    public void CheckFromFen(string fen, int depth, int expectedNodes, int expectedCaptures)
    {
        var board = BoardBuilder.FromFen(fen);
        var (captures, count) = GetPossibleMovesCount(board, depth);

        Assert.Equal(expectedCaptures, captures);
        Assert.Equal(expectedNodes, count);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    // [InlineData(6)]
    // [InlineData(7)]
    // [InlineData(8)]
    // [InlineData(9)]
    public void Check(int depth)
    {
        var board = BoardBuilder.GetDefaultStartingPosition();
        var (captures, count) = GetPossibleMovesCount(board, depth);

        Assert.Equal(depthToCount[depth], count);
    }

    private (long, long) GetPossibleMovesCount(Board board, int toDepth)
    {
        if (toDepth == 0)
        {
            // _testOutputHelper.WriteLine(board.ToString());
            return (0, 1);
        }

        var moves = board.GetLegalMoves();
        long nodes = 0;
        long captures = 0;
        foreach(var move in moves)
        {
            var boardCopy = board.Clone();
            board.MakeMove(move);
            var (cap, nod) = GetPossibleMovesCount(board, toDepth - 1);
            captures += cap;
            nodes += nod;
            if (toDepth == 1 && move.CapturedPiece is not null)
                captures++;
            board.UndoLastMove();
            if (!boardCopy.Equals(board))
            {
                boardCopy.MakeMove(move);
                boardCopy.UndoLastMove();
            }
        }

        return (captures, nodes);
    }
}
