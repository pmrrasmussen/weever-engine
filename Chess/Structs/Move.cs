using Chess.Enums;

namespace Chess.Structs;

public readonly struct Move(
    Square from,
    Square to,
    Piece promotionTo = Piece.None)
{
    public readonly Square From = from;

    public readonly Square To = to;

    public readonly Piece PromotionTo = promotionTo;

    public int Length()
    {
        var moveDelta = To - From;
        return moveDelta.Length();
    }

    public override string ToString()
    {
        return $"{From}{To}";
    }
}

public static class MoveExtensions
{
    public static Move ToMove(this string move)
    {
        if (move.Length is < 4 or > 5)
            throw new ArgumentOutOfRangeException($"Unknown move {move}");

        var fromSquare = move[..2].ToSquare();
        var toSquare = move[2..4].ToSquare();

        if (move.Length == 4)
            return new Move(fromSquare, toSquare);

        var promotionPiece = move[5].ToPiece();
        promotionPiece |= toSquare.Y == 1 ? Piece.White : Piece.Black;

        return new Move(fromSquare, toSquare, promotionPiece);
    }
}
