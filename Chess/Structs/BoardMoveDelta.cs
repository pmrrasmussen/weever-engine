using Chess.Enums;

namespace Chess.Structs;

public readonly struct BoardMoveDelta(
    Move move,
    CastlingPrivileges castlingPrivileges,
    Square enPassantAttackSquare,
    int middleGameEvaluation,
    int endGameEvaluation,
    int gamePhase,
    Piece directlyCapturedPiece = Piece.Empty)
{
    public readonly Move Move = move;

    public readonly CastlingPrivileges CastlingPrivileges = castlingPrivileges;

    public readonly Square EnPassantAttackSquare = enPassantAttackSquare;

    public readonly Piece DirectlyCapturedPiece = directlyCapturedPiece;

    public readonly int MiddleGameEvaluation = middleGameEvaluation;

    public readonly int EndGameEvaluation = endGameEvaluation;

    public readonly int GamePhase = gamePhase;
}
