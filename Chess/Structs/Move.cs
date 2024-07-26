namespace Chess.Structs;

public readonly struct Move(
    Square from,
    Square to,
    Piece movedPiece,
    Piece? capturedPiece = null,
    Piece? promotionTo = null)
{
    public readonly Square From = from;

    public readonly Square To = to;

    public readonly Piece MovedPiece = movedPiece;

    public readonly Piece? CapturedPiece;

    public readonly Piece? PromotionTo = promotionTo;

    public int Length()
    {
        var moveDelta = To - From;
        return moveDelta.Length();
    }
}
