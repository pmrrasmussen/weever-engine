// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Chess;
using Chess.Builders;

namespace Engine;

public class Program
{
    public static void Main()
    {
        long count = 0;
        for (int i = 0; i < 10; i++)
        {
            var board = BoardBuilder.GetDefaultStartingPosition();
            var start = Stopwatch.GetTimestamp();

            (_, count) = GetPossibleMovesCount(board, 5);

            Console.WriteLine(Stopwatch.GetElapsedTime(start));
        }

        Console.WriteLine(count);
    }
    private static (long, long) GetPossibleMovesCount(Board board, int toDepth)
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
            board.MakeMove(move);
            var (cap, nod) = GetPossibleMovesCount(board, toDepth - 1);
            captures += cap;
            nodes += nod;
            if (toDepth == 1 && move.CapturedPiece is not null)
                captures++;
            board.UndoLastMove();
        }

        return (captures, nodes);
    }
}
