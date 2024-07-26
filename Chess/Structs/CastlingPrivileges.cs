namespace Chess.Structs;

public readonly struct CastlingPrivileges(
    bool whiteKingSide,
    bool whiteQueenSide,
    bool blackKingSide,
    bool blackQueenSide)
{
    public readonly bool WhiteKingSide  = whiteKingSide;

    public readonly bool WhiteQueenSide = whiteQueenSide;

    public readonly bool BlackKingSide  = blackKingSide;

    public readonly bool BlackQueenSide = blackQueenSide;
}
