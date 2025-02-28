using Chess.Enums;
using Chess.Exceptions;

namespace Chess.Structs;

public readonly struct Move(
    Square from,
    Square to,
    Piece promotionTo = Piece.Empty)
{
    public readonly Square From = from;

    public readonly Square To = to;

    public readonly Piece PromotionTo = promotionTo;

    public override string ToString()
    {
        var promotionString = PromotionTo switch
        {
            Piece.Empty                                             => "",
            var piece when (piece & Piece.TypeMask) == Piece.Queen  => "q",
            var piece when (piece & Piece.TypeMask) == Piece.Rook   => "r",
            var piece when (piece & Piece.TypeMask) == Piece.Bishop => "b",
            var piece when (piece & Piece.TypeMask) == Piece.Knight => "n",
            _ => throw new InvalidMoveException($"Invalid promotion type for move: {PromotionTo}")
        };

        return $"{From}{To}{promotionString}".ToLower();
    }
}

public static class MoveExtensions
{
    public static Move ToMove(this string move)
    {
        if (move.Length is < 4 or > 5)
            throw new InvalidMoveException($"Unknown move {move}");

        var fromSquare = move[..2].ToSquare();
        var toSquare = move[2..4].ToSquare();

        if (move.Length == 4)
            return new Move(fromSquare, toSquare);

        var promotionPiece = move[4].ToPiece();
        promotionPiece |= (int)toSquare/20 == 2 ? Piece.Black : Piece.White;

        return new Move(fromSquare, toSquare, promotionPiece);
    }
}
