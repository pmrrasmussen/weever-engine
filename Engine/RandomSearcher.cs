using Chess;
using Chess.Builders;
using Chess.Structs;
using Engine.Interfaces;

namespace Engine;

public class RandomSearcher : ISearcher
{
    private Board _board = BoardBuilder.GetDefaultStartingPosition();
    private Random _random = new Random();

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
        var index = _random.Next(moves.Count);

        return moves[index];
    }
}
