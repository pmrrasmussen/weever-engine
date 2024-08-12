namespace Chess.Enums;

[Flags]
public enum CastlingPrivileges
{
    None           = 0,
    WhiteKingSide  = 1 << 0,
    WhiteQueenSide = 1 << 1,
    BlackKingSide  = 1 << 2,
    BlackQueenSide = 1 << 3,
    White = WhiteKingSide | WhiteQueenSide,
    Black = BlackKingSide | BlackQueenSide,
    All = White | Black,
}
