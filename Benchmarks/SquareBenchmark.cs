using BenchmarkDotNet.Attributes;
using Chess.Structs;

namespace Benchmarks;

public class SquareBenchmark
{
    private Square _square;

    public SquareBenchmark()
    {
        _square = new Square(X, Y);
    }

    [Params(-1)]
    public int X { get; set; }

    [Params(5)]
    public int Y{ get; set; }

    [Benchmark]
    public void NaiveSquareImplementation()
    {
        _square.IsWithinBoard();
    }
}
