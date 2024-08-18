namespace Chess.Enums;

[Flags]
public enum Piece
{
    Pawn   = 0,
    Knight = 1,
    Bishop = 2,
    Rook   = 3,
    Queen  = 4,
    King   = 5,
    Empty   = 6,
    White  = 1 << 3,
    Black  = 1 << 4,
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
    TypeMask = 7,
    ColorMask = 3 << 3,
    OutOfBounds = 1 << 5,
}

public static class PieceExtensions
{
    public static int KingPositionIndex(this Piece color)
        => (int)color >> 4;

    public static string AsString(this Piece piece)
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
            Piece.Empty => "\u00B7",
            _ => throw new ArgumentOutOfRangeException($"Unknown piece {piece}"),
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

    public static int MiddleGameValue(this Piece piece)
    {
        return piece switch
        {
            Piece.WhiteKing => 0,
            Piece.BlackKing => 0,
            Piece.WhiteQueen => 1025,
            Piece.BlackQueen => -1025,
            Piece.WhiteRook => 477,
            Piece.BlackRook => -477,
            Piece.WhiteBishop => 365,
            Piece.BlackBishop => -365,
            Piece.WhiteKnight => 337,
            Piece.BlackKnight => -337,
            Piece.WhitePawn => 82,
            Piece.BlackPawn => -82,
            Piece.Empty => 0,
            _ => throw new ArgumentOutOfRangeException($"Unknown piece {piece}"),
        };
    }

    public static int EndGameValue(this Piece piece)
    {
        return piece switch
        {
            Piece.WhiteKing => 0,
            Piece.BlackKing => 0,
            Piece.WhiteQueen => 936,
            Piece.BlackQueen => -936,
            Piece.WhiteRook => 512,
            Piece.BlackRook => -512,
            Piece.WhiteBishop => 297,
            Piece.BlackBishop => -297,
            Piece.WhiteKnight => 281,
            Piece.BlackKnight => -281,
            Piece.WhitePawn => 94,
            Piece.BlackPawn => -94,
            Piece.Empty => 0,
            _ => throw new ArgumentOutOfRangeException($"Unknown piece {piece}"),
        };
    }

    public static int GamePhaseValue(this Piece piece)
    {
        return piece switch
        {
            Piece.WhiteKing => 0,
            Piece.BlackKing => 0,
            Piece.WhiteQueen => 4,
            Piece.BlackQueen => 4,
            Piece.WhiteRook => 2,
            Piece.BlackRook => 2,
            Piece.WhiteBishop => 1,
            Piece.BlackBishop => 1,
            Piece.WhiteKnight => 1,
            Piece.BlackKnight => 1,
            Piece.WhitePawn => 0,
            Piece.BlackPawn => 0,
            Piece.Empty => 0,
            _ => throw new ArgumentOutOfRangeException($"Unknown piece {piece}"),
        };
    }
}
