using Chess.Enums;
using Chess.Structs;

namespace Chess;

public partial class Board
{
    public void MakeMove(Move move)
    {
        _moveHistory.Push(new BoardMoveDelta(
            move: move,
            castlingPrivileges: _castlingPrivileges,
            enPassantAttackSquare: _enPassantAttackSquare,
            middlegameEvaluation: _middlegameEvaluation,
            endgameEvaluation: _endgameEvaluation,
            gamePhase: _gamePhase,
            directlyCapturedPiece: this[move.To]));

        var movedPiece = this[move.From];
        var movedPieceType = movedPiece & Piece.TypeMask;

        HandleEnPassantCaptureWhenMakingMove(move, movedPieceType);
        HandleCastlingWhenMakingMove(move, movedPieceType);
        SetCastlingPrivilegesWhenMakingMove(move, movedPieceType);
        _enPassantAttackSquare = NewEnPassantSquareWhenMakingMove(move, movedPieceType);

        var pieceToPlace = move.PromotionTo !=  Piece.Empty ? move.PromotionTo : movedPiece;

        UpdateGamePhase(move.To, movedPiece, pieceToPlace);

        EvaluateReplacingPieceOnSquare(move.From, Piece.Empty);
        EvaluateReplacingPieceOnSquare(move.To, pieceToPlace);

        this[move.From] = Piece.Empty;
        this[move.To] = pieceToPlace;

        _colorToMove ^= Piece.ColorMask;
    }

    public void UndoLastMove()
    {
        var moveDelta = _moveHistory.Pop();

        var movedPiece = this[moveDelta.Move.To];
        var movedPieceType = movedPiece & Piece.TypeMask;

        _castlingPrivileges = moveDelta.CastlingPrivileges;
        _enPassantAttackSquare = moveDelta.EnPassantAttackSquare;
        _middlegameEvaluation = moveDelta.MiddlegameEvaluation;
        _endgameEvaluation = moveDelta.EndgameEvaluation;
        _gamePhase = moveDelta.GamePhase;
        _colorToMove ^= Piece.ColorMask;

        var lastMove = moveDelta.Move;

        HandleUndoEnPassant(lastMove, movedPieceType);
        HandleUndoCastling(lastMove, movedPieceType);

        this[lastMove.From] = moveDelta.Move.PromotionTo == Piece.Empty
            ? movedPiece
            : Piece.Pawn | _colorToMove;
        this[lastMove.To] = moveDelta.DirectlyCapturedPiece;
    }

    private void HandleUndoEnPassant(Move move, Piece movedPieceType)
    {
        if ( movedPieceType != Piece.Pawn || move.To != _enPassantAttackSquare)
            return;

        var moveDirectionForOtherColor = _colorToMove == Piece.White ? Down : Up;
        this[_enPassantAttackSquare + moveDirectionForOtherColor] = Piece.Pawn ^ _colorToMove ^ Piece.ColorMask;
    }

    private void HandleUndoCastling(Move move, Piece movedPieceType)
    {
        if (movedPieceType != Piece.King ||
            Math.Abs(move.From-move.To) != 2)
            return;

        var horizontalMoveDelta = move.To - move.From;

        var originalRookPosition = (Square)(horizontalMoveDelta > 0 ? 8 : 1) + 10 * move.From.GetRank();
        var currentRookPosition = (Square)(((int)move.To + (int)move.From) / 2);

        this[originalRookPosition] = this[currentRookPosition];
        this[currentRookPosition] = Piece.Empty;
    }

    private void HandleEnPassantCaptureWhenMakingMove(Move move, Piece movedPieceType)
    {
        if (movedPieceType != Piece.Pawn || move.To != _enPassantAttackSquare)
            return;

        var moveDirectionForOtherColor = _colorToMove == Piece.White ? Down : Up;

        var enPassantCaptureSquare = _enPassantAttackSquare + moveDirectionForOtherColor;
        EvaluateReplacingPieceOnSquare(enPassantCaptureSquare, Piece.Empty);
        this[enPassantCaptureSquare] = Piece.Empty;
    }

    private Square NewEnPassantSquareWhenMakingMove(Move move, Piece movedPieceType)
    {
        return movedPieceType == Piece.Pawn && Math.Abs(move.To - move.From) == 20
            ? (Square)(((int)move.To + (int)move.From) / 2)
            : default;
    }

    private void HandleCastlingWhenMakingMove(Move move, Piece movedPieceType)
    {
        if (movedPieceType != Piece.King || Math.Abs(move.From-move.To) != 2)
            return;

        var horizontalMoveDelta = move.To - move.From;

        var currentRookPosition = (Square)(horizontalMoveDelta > 0 ? 8 : 1) + 10 * move.From.GetRank();
        var newRookPosition = (Square)(((int)move.To + (int)move.From) / 2);

        EvaluateReplacingPieceOnSquare(newRookPosition, this[currentRookPosition]);
        EvaluateReplacingPieceOnSquare(currentRookPosition, Piece.Empty);
        this[newRookPosition] = this[currentRookPosition];
        this[currentRookPosition] = Piece.Empty;
    }

    private void SetCastlingPrivilegesWhenMakingMove(Move move, Piece movedPieceType)
    {
        if (movedPieceType == Piece.King)
        {
            if (_colorToMove == Piece.White)
                _castlingPrivileges &= CastlingPrivileges.Black;
            else
                _castlingPrivileges &= CastlingPrivileges.White;
        }

        if (move.From == Square.A1 || move.To == Square.A1)
            _castlingPrivileges &= CastlingPrivileges.Black | CastlingPrivileges.WhiteKingSide;

        if (move.From == Square.H1 || move.To == Square.H1)
            _castlingPrivileges &= CastlingPrivileges.Black | CastlingPrivileges.WhiteQueenSide;

        if (move.From == Square.A8 || move.To == Square.A8)
            _castlingPrivileges &= CastlingPrivileges.White | CastlingPrivileges.BlackKingSide;

        if (move.From == Square.H8 || move.To == Square.H8)
            _castlingPrivileges &= CastlingPrivileges.White | CastlingPrivileges.BlackQueenSide;
    }
}
