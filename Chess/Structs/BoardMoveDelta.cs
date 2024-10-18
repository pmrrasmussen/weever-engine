using Chess.Enums;

namespace Chess.Structs;

public readonly struct BoardMoveDelta(
    Move move,
    CastlingPrivileges castlingPrivileges,
    Square enPassantAttackSquare,
    int middlegameEvaluation,
    int endgameEvaluation,
    int gamePhase,
    long boardHash,
    Piece directlyCapturedPiece = Piece.Empty)
{
    public readonly Move Move = move;

    public readonly CastlingPrivileges CastlingPrivileges = castlingPrivileges;

    public readonly Square EnPassantAttackSquare = enPassantAttackSquare;

    public readonly Piece DirectlyCapturedPiece = directlyCapturedPiece;

    public readonly int MiddlegameEvaluation = middlegameEvaluation;

    public readonly int EndgameEvaluation = endgameEvaluation;

    public readonly int GamePhase = gamePhase;

    public readonly long BoardHash = boardHash;

    public override bool Equals(object? obj)
    {
        return obj is BoardMoveDelta otherMove
            && Move.Equals(otherMove.Move)
            && CastlingPrivileges == otherMove.CastlingPrivileges
            && EnPassantAttackSquare == otherMove.EnPassantAttackSquare
            && DirectlyCapturedPiece == otherMove.DirectlyCapturedPiece
            && MiddlegameEvaluation == otherMove.MiddlegameEvaluation
            && EndgameEvaluation == otherMove.EndgameEvaluation
            && GamePhase == otherMove.GamePhase;
    }
}
