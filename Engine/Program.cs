// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Chess;
using Chess.Builders;

namespace Engine;

public class Program
{
    public static void Main()
    {
        long nodes = 0;
        for (int i = 0; i < 10; i++)
        {
            var board = BoardBuilder.GetDefaultStartingPosition();
            var start = Stopwatch.GetTimestamp();

            nodes = GetNodeCount(board, 6);

            Console.WriteLine(Stopwatch.GetElapsedTime(start));
        }

        Console.WriteLine(nodes);
    }
    private static long GetNodeCount(Board board, int depth)
    {
        if (depth == 0)
            return 1;

        var moves = board.GetLegalMoves();
        long nodes = 0;
        foreach(var move in moves)
        {
            board.MakeMove(move);
            nodes += GetNodeCount(board, depth - 1);
            board.UndoLastMove();
        }

        return nodes;
    }
}
