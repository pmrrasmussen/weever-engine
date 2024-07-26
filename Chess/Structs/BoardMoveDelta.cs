namespace Chess.Structs;

public readonly struct BoardMoveDelta(
    Move move,
    CastlingPrivileges castlingPrivileges,
    Square enPassantAttackSquare)
{
    public readonly Move Move = move;

    public readonly CastlingPrivileges CastlingPrivileges = castlingPrivileges;

    public readonly Square EnPassantAttackSquare = enPassantAttackSquare;
}
