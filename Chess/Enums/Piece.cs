namespace Chess.Enums;

[Flags]
public enum Piece : byte
{
    None   = 0,
    White  = 1 << 0,
    Black  = 1 << 1,
    Pawn   = 1 << 2,
    Knight = 1 << 3,
    Bishop = 1 << 4,
    Rook   = 1 << 5,
    Queen  = 1 << 6,
    King   = 1 << 7,
    BlackKing   = Black | King,
    BlackQueen  = Black | Queen,
    BlackRook   = Black | Rook,
    BlackBishop = Black | Bishop,
    BlackKnight = Black | Knight,
    BlackPawn   = Black | Pawn,
    WhitePawn   = White | Pawn,
    WhiteKnight = White | Knight,
    WhiteBishop = White | Bishop,
    WhiteRook   = White | Rook,
    WhiteQueen  = White | Queen,
    WhiteKing   = White | King,
}

public static class PieceExtensions
{
    public static string PieceToString(Piece piece)
    {
        return piece switch
        {
            Piece.WhiteKing => "\u2654",
            Piece.BlackKing => "\u265a",
            Piece.WhiteQueen => "\u2655",
            Piece.BlackQueen => "\u265b",
            Piece.WhiteRook => "\u2656",
            Piece.BlackRook => "\u265c",
            Piece.WhiteBishop => "\u2657",
            Piece.BlackBishop => "\u265d",
            Piece.WhiteKnight => "\u2658",
            Piece.BlackKnight => "\u265e",
            Piece.WhitePawn => "\u2659",
            Piece.BlackPawn => "\u265f",
            Piece.None => "\u00B7",
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public static Piece ToPiece(this char piece)
    {
        return piece switch
        {
            'Q' or 'q' => Piece.Queen,
            'R' or 'r' => Piece.Rook,
            'B' or 'b' => Piece.Bishop,
            'N' or 'n' => Piece.Knight,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
