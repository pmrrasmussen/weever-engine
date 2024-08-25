using System.Security;
using Chess;
using Chess.Builders;
using Chess.Structs;
using Engine.Interfaces;

namespace Engine;

public class AlphaBetaSearcher : ISearcher
{
    private const int Depth = 6;
    private const int MaxScore = 100000;
    private const int MinScore = -MaxScore;


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
        var depth = 1;
        Move bestMove = default;

        // Iterative deepening
        while (true)
        {
            Console.WriteLine(depth);
            var (candidateBestMove, _) = AlphaBetaSearch(
                position: _board,
                alpha: MinScore,
                beta: MaxScore,
                remainingDepth: depth++,
                cancellationToken: cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                break;

            bestMove = candidateBestMove;
        }

        return bestMove;
    }

    private (Move bestMove, int evaluation) AlphaBetaSearch(
        Board position,
        int alpha,
        int beta,
        int remainingDepth,
        CancellationToken cancellationToken)
    {
        Move bestMove = default;

        if (remainingDepth == 0)
            return (bestMove, position.GetEvaluation());

        var bestEvaluation = MinScore-remainingDepth;

        foreach (var move in position.GetLegalMoves())
        {
            position.MakeMove(move);
            var (_, evaluation) = AlphaBetaSearch(
                position: position,
                alpha: -beta,
                beta: -alpha,
                remainingDepth: remainingDepth - 1,
                cancellationToken: cancellationToken);
            position.UndoLastMove();

            if (-evaluation > bestEvaluation)
            {
                alpha = int.Max(bestEvaluation = -evaluation, alpha);
                bestMove = move;
            }

            if (bestEvaluation >= beta || cancellationToken.IsCancellationRequested)
                break;
        }

        return (bestMove, bestEvaluation);
    }
}
