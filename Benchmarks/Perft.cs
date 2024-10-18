using BenchmarkDotNet.Attributes;
using Chess;
using Chess.Builders;
using Chess.ZobristHashing;
using Microsoft.CodeAnalysis;

namespace Benchmarks;

public class Perft
{
    private Board _board;
    private ForgetfulHashMap<(int depth, long count)> _transpositionTable;

    public Perft()
    {
        _board = BoardBuilder.GetDefaultStartingPosition();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _transpositionTable = new ForgetfulHashMap<(int depth, long count)>(5000000);
    }

    [Params(6)]
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

        if (_transpositionTable.TryGetValue(board.ZobristHash, out var element))
        {
            if (element.depth == toDepth)
                return element.count;
        }

        var moves = board.GetLegalMoves();
        long sum = 1;
        foreach(var move in moves)
        {
            board.MakeMove(move);
            sum += GetNodeCount(board, toDepth - 1);
            board.UndoLastMove();
        }

        _transpositionTable.Add((toDepth, sum), board.ZobristHash);

        return sum;
    }
}
