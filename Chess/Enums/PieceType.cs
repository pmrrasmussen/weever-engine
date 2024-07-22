namespace Chess.Enums;

public enum PieceType : byte
{
    Pawn   = 1 << 1,
    Knight = 1 << 2,
    Bishop = 1 << 3,
    Rook   = 1 << 4,
    Queen  = 1 << 5,
    King   = 1 << 6,
    SlidingPieces = Bishop | Rook | Queen,
}
