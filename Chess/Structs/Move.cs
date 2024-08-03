namespace Chess.Structs;

public readonly struct Move(
    Square from,
    Square to,
    Piece? capturedPiece = null,
    Piece? promotionTo = null)
{
    public readonly Square From = from;

    public readonly Square To = to;

    public readonly Piece? CapturedPiece = capturedPiece;

    public readonly Piece? PromotionTo = promotionTo;

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
