using Chess.Enums;

namespace Chess.Structs;

public readonly struct BoardMoveDelta(
    Move move,
    CastlingPrivileges castlingPrivileges,
    Square enPassantAttackSquare,
    int middlegameEvaluation,
    int endgameEvaluation,
    int gamePhase,
    Piece directlyCapturedPiece = Piece.Empty)
{
    public readonly Move Move = move;

    public readonly CastlingPrivileges CastlingPrivileges = castlingPrivileges;

    public readonly Square EnPassantAttackSquare = enPassantAttackSquare;

    public readonly Piece DirectlyCapturedPiece = directlyCapturedPiece;

    public readonly int MiddlegameEvaluation = middlegameEvaluation;

    public readonly int EndgameEvaluation = endgameEvaluation;

    public readonly int GamePhase = gamePhase;
}
