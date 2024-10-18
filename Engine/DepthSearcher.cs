using System.Collections;
using Chess;
using Chess.Builders;
using Chess.Structs;
using Chess.ZobristHashing;
using Engine.Interfaces;

namespace Engine;

public class DepthSearcher : ISearcher
{
    private const int Depth = 6;
    private const int MinScore = -100000;
    private const int TranspositionTableSize = 1000000;

    private Board _board = BoardBuilder.GetDefaultStartingPosition();
    private ForgetfulHashMap<(int depth, int evaluation)> _transpositionTable = new (TranspositionTableSize);

    public void SetPosition(string fen, IEnumerable<Move> moves)
    {
        _board = string.IsNullOrEmpty(fen)
            ? BoardBuilder.GetDefaultStartingPosition()
            : BoardBuilder.FromFen(fen);

        foreach (var move in moves)
            _board.MakeMove(move);
    }

    public (Move move, int evaluation) Search(int remainingTimeInMilliseconds, CancellationToken cancellationToken)
    {
        var moves = _board.GetLegalMoves();
        var bestScore = MinScore-Depth;
        var bestMove = moves[0];

        foreach (var move in moves)
        {
            _board.MakeMove(move);
            var evaluation = -GetEvaluation(_board, Depth-1);
            _board.UndoLastMove();

            if (bestScore >= evaluation)
                continue;

            bestScore = evaluation;
            bestMove = move;
        }

        return (bestMove, bestScore);
    }

    private int GetEvaluation(Board position, int depth)
    {
        if (depth == 0)
            return position.GetEvaluation();

        if (depth > 2)
        {
            if (_transpositionTable.TryGetValue(_board.ZobristHash, out var tableEntry))
            {
                if (tableEntry.depth >= depth)
                    return tableEntry.evaluation;
            }
        }

        var best = MinScore-depth;

        foreach (var move in position.GetLegalMoves())
        {
            position.MakeMove(move);
            best = int.Max(best, -GetEvaluation(position, depth - 1));
            position.UndoLastMove();
        }

        if (depth > 2)
        {
            _transpositionTable.Add((depth, best), _board.ZobristHash);
        }

        return best;
    }
}
