// See https://aka.ms/new-console-template for more information

using Chess;
using Chess.Builders;
using Chess.Structs;

namespace PerftExplorer;

public static class Program
{
    public static void Main(string[] args)
    {
        if (!int.TryParse(args[0], out var depth))
            Console.WriteLine("Must pass an integer for the depth argument.");

        var fen = args[1];

        var moves = new List<Move>();

        if (args.Length >= 3)
             moves.AddRange(args[2].Split().Select(moveString => moveString.ToMove()));

        var board = BoardBuilder.FromFen(fen);

        foreach (var move in moves)
            board.MakeMove(move);

        long total = 0;

        foreach (var move in board.GetLegalMoves())
        {
            board.MakeMove(move);
            var nodeCount = GetNodeCount(board, depth - 1);
            board.UndoLastMove();
            Console.WriteLine($"{move.ToString().ToLower()} {nodeCount}");

            total += nodeCount;
        }

        Console.WriteLine($"\n{total}");
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
