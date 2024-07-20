using Chess.Enums;

namespace Chess.Structs;

public readonly struct Move(Square from, Square to, Piece? promotionTo = null)
{
    public readonly Square From = from;

    public readonly Square To = to;

    public readonly Piece? PromotionTo = promotionTo;
}
