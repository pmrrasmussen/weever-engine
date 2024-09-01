using Chess;
using Chess.Builders;
using Chess.Structs;
using Engine.Interfaces;

namespace Engine;

public class GreedySearcher : ISearcher
{
    private Board _board = BoardBuilder.GetDefaultStartingPosition();

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
        var bestScore = int.MinValue;
        var bestMove = moves[0];

        var scoreMultiplier = _board.WhiteToMove ? 1 : -1;

        foreach (var move in moves)
        {
            _board.MakeMove(move);
            var evaluation = scoreMultiplier * _board.GetEvaluation();
            _board.UndoLastMove();

            if (bestScore >= evaluation)
                continue;

            bestScore = evaluation;
            bestMove = move;
        }

        return (bestMove, bestScore);
    }
}
