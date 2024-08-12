using BenchmarkDotNet.Attributes;
using Chess.Enums;

namespace Benchmarks;

public class PieceBenchmark
{
    private Piece _piece;

    public PieceBenchmark()
    {
        _piece = Piece.BlackQueen;
    }

    [Benchmark]
    public bool HasFlagNegative()
    {
        var result = false;
        for (int i = 0; i < 200; i++)
        {
            result |= _piece.HasFlag(Piece.White);

        }

        return result;
    }

    [Benchmark]
    public bool HasFlagPositive()
    {
        var result = false;
        for (int i = 0; i < 200; i++)
        {
            result |= _piece.HasFlag(Piece.Black);

        }

        return result;
    }

    [Benchmark]
    public bool AndNegative()
    {
        var result = false;
        for (int i = 0; i < 200; i++)
        {
            result |= (_piece & Piece.White) != 0;

        }

        return result;
    }

    [Benchmark]
    public bool AndPositive()
    {
        var result = false;
        for (int i = 0; i < 200; i++)
        {
            result |= (_piece & Piece.Black) != 0;
        }

        return result;
    }
}
