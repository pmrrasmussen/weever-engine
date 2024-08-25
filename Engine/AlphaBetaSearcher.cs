using Chess;
using Chess.Builders;
using Chess.Structs;
using Engine.Interfaces;

namespace Engine;

public class AlphaBetaSearcher : ISearcher
{
    public const int CheckMateScore = 100000;
    private const int MinScore = -CheckMateScore;

    private Board _board;
    private int _maxPly;

    public AlphaBetaSearcher(int maxPly = int.MaxValue)
    {
        _board = BoardBuilder.GetDefaultStartingPosition();
        _maxPly = maxPly;
    }

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
        var depth = 1;
        Move bestMove = default;
        int bestEvaluation = default;

        // Iterative deepening
        while (depth <= _maxPly)
        {
            var (bestMoves, evaluation) = AlphaBetaSearch(
                position: _board,
                alpha: MinScore - depth,
                beta: CheckMateScore + depth,
                remainingDepth: depth++,
                cancellationToken: cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                break;

            Console.WriteLine($"info depth {depth} score cp {evaluation} pv {String.Join(' ', bestMoves.Select(m => m.ToString().ToLower()).Reverse())}");
            bestMove = bestMoves[^1];
            bestEvaluation = evaluation;
        }

        return (bestMove, bestEvaluation);
    }

    private (List<Move> bestMove, int evaluation) AlphaBetaSearch(
        Board position,
        int alpha,
        int beta,
        int remainingDepth,
        CancellationToken cancellationToken)
    {
        // TODO: Something is wrong in the endgame near checkmate. Check when the max value is applied.
        var bestVariation = new List<Move>();
        var variation = new List<Move>();
        int moveEvaluation;

        var bestEvaluation = MinScore-remainingDepth;

        var legalMoves = position.GetLegalMoves();
        if (legalMoves.Count == 0)
            return ([], _board.IsPlayerToMoveInCheck() ? bestEvaluation : 0);

        foreach (var move in legalMoves)
        {
            position.MakeMove(move);
            (variation, moveEvaluation) = remainingDepth == 0
                ? (variation, _board.GetEvaluation())
                : AlphaBetaSearch(
                position: position,
                alpha: -beta,
                beta: -alpha,
                remainingDepth: remainingDepth - 1,
                cancellationToken: cancellationToken);
            position.UndoLastMove();

            if (-moveEvaluation > bestEvaluation)
            {
                bestEvaluation = -moveEvaluation;
                alpha = int.Max(bestEvaluation, alpha);
                bestVariation = variation.ToList();
                bestVariation.Add(move);
            }

            if (bestEvaluation >= beta || cancellationToken.IsCancellationRequested)
                break;
        }

        return (bestVariation, bestEvaluation);
    }
}
