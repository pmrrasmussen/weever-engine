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
