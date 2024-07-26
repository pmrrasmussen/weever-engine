using BenchmarkDotNet.Attributes;
using Chess;
using Chess.Builders;

namespace Benchmarks;

public class Perft
{
    private Board _board;

    public Perft()
    {
        _board = BoardBuilder.GetDefaultStartingPosition();
    }

    [Params(9)]
    public int Depth { get; set; }

    [Benchmark]
    public long NodeSearch()
    {
        return GetNodeCount(_board, Depth);
    }

    private long GetNodeCount(Board board, int toDepth)
    {
        if (toDepth == 0)
            return 1;

        var moves = board.GetPseudoLegalMoves();
        long sum = 1;
        foreach(var move in moves)
        {
            board.MakeMove(move);
            sum += GetNodeCount(board, toDepth - 1);
            board.UndoLastMove();
        }

        return sum;
    }
}
