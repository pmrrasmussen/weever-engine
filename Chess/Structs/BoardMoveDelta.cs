using Chess.Enums;

namespace Chess.Structs;

public readonly struct BoardMoveDelta(
    Move move,
    CastlingPrivileges castlingPrivileges,
    Square enPassantAttackSquare,
    Piece directlyCapturedPiece = Piece.None)
{
    public readonly Move Move = move;

    public readonly CastlingPrivileges CastlingPrivileges = castlingPrivileges;

    public readonly Square EnPassantAttackSquare = enPassantAttackSquare;

    public readonly Piece DirectlyCapturedPiece = directlyCapturedPiece;
}
