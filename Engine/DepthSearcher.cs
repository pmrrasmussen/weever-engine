using Chess;
using Chess.Builders;
using Chess.Structs;
using Engine.Interfaces;

namespace Engine;

public class DepthSearcher : ISearcher
{
    private const int Depth = 5;
    private const int MinScore = -100000;

    private Board _board = BoardBuilder.GetDefaultStartingPosition();

    public void SetPosition(string fen, IEnumerable<Move> moves)
    {
        _board = string.IsNullOrEmpty(fen)
            ? BoardBuilder.GetDefaultStartingPosition()
            : BoardBuilder.FromFen(fen);

        foreach (var move in moves)
            _board.MakeMove(move);
    }

    public Move Search(int remainingTimeInMilliseconds, CancellationToken cancellationToken)
    {
        var moves = _board.GetLegalMoves();
        var bestScore = MinScore-Depth;
        var bestMove = moves[0];

        var colorMultiplier = _board.WhiteToMove ? 1 : -1;

        foreach (var move in moves)
        {
            _board.MakeMove(move);
            var evaluation = colorMultiplier * GetEvaluation(_board, Depth-1);
            _board.UndoLastMove();

            if (bestScore >= evaluation)
                continue;

            bestScore = evaluation;
            bestMove = move;
        }

        return bestMove;
    }

    private int GetEvaluation(Board position, int depth)
    {
        if (depth == 0)
            return position.GetEvaluation();

        var best = MinScore-depth;
        var colorMultiplier = position.WhiteToMove ? 1 : -1;

        foreach (var move in position.GetLegalMoves())
        {
            position.MakeMove(move);
            best = int.Max(best, GetEvaluation(position, depth - 1) * colorMultiplier);
            position.UndoLastMove();
        }

        return best * colorMultiplier;
    }
}
