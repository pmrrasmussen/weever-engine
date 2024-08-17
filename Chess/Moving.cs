using Chess.Enums;
using Chess.Structs;

namespace Chess;

public partial class Board
{
    public void MakeMove(Move move)
    {
        _moveHistory.Push(new BoardMoveDelta(
            move,
            _castlingPrivileges,
            _enPassantAttackSquare,
            directlyCapturedPiece: this[move.To]));

        var movedPiece = this[move.From];
        var movedPieceType = movedPiece & Piece.TypeMask;

        HandleEnPassantCaptureWhenMakingMove(move, movedPieceType);
        HandleCastlingWhenMakingMove(move, movedPieceType);
        SetCastlingPrivilegesWhenMakingMove(move, movedPieceType);
        _enPassantAttackSquare = NewEnPassantSquareWhenMakingMove(move, movedPieceType);

        var pieceToPlace = move.PromotionTo !=  Piece.Empty ? move.PromotionTo : movedPiece;
        this[move.From] = Piece.Empty;
        this[move.To] = pieceToPlace;

        if (movedPieceType is Piece.King)
        {
            _kingPositions[_colorToMove.KingPositionIndex()] = move.To;
        }

        _colorToMove = _colorToMove == Piece.White ? Piece.Black : Piece.White;
    }

    public void UndoLastMove()
    {
        var moveDelta = _moveHistory.Pop();

        var movedPiece = this[moveDelta.Move.To];
        var movedPieceType = movedPiece & Piece.TypeMask;

        _castlingPrivileges = moveDelta.CastlingPrivileges;
        _enPassantAttackSquare = moveDelta.EnPassantAttackSquare;
        _colorToMove = _colorToMove == Piece.White ? Piece.Black : Piece.White;

        var lastMove = moveDelta.Move;

        HandleUndoEnPassant(lastMove, movedPieceType);
        HandleUndoCastling(lastMove, movedPieceType);

        this[lastMove.From] = moveDelta.Move.PromotionTo == Piece.Empty
            ? movedPiece
            : Piece.Pawn | _colorToMove;
        this[lastMove.To] = moveDelta.DirectlyCapturedPiece;

        if (movedPieceType is Piece.King)
        {
            _kingPositions[_colorToMove.KingPositionIndex()] = lastMove.From;
        }
    }

    private void HandleUndoEnPassant(Move move, Piece movedPieceType)
    {
        if (movedPieceType != Piece.Pawn || _enPassantAttackSquare == Square.NullSquare)
            return;

        var moveDirectionForOtherColor = _colorToMove == Piece.White ? Down : Up;

        if (move.To == _enPassantAttackSquare)
        {
            var otherColor = _colorToMove == Piece.White ? Piece.Black : Piece.White;
            this[_enPassantAttackSquare + moveDirectionForOtherColor] = Piece.Pawn | otherColor;
        }

    }

    private void HandleUndoCastling(Move move, Piece movedPieceType)
    {
        if (movedPieceType != Piece.King ||
            Math.Abs(move.From-move.To) != 2)
            return;

        var horizontalMoveDelta = (move.To - move.From) % 10;

        var originalRookPosition = (Square)(horizontalMoveDelta > 0 ? 8 : 1) + 10 * move.From.Rank();
        var currentRookPosition = move.From + horizontalMoveDelta / 2;

        this[originalRookPosition] = this[currentRookPosition];
        this[currentRookPosition] = Piece.Empty;
    }

    private void HandleEnPassantCaptureWhenMakingMove(Move move, Piece movedPieceType)
    {
        if (movedPieceType != Piece.Pawn || _enPassantAttackSquare == Square.NullSquare)
            return;

        var moveDirectionForOtherColor = _colorToMove == Piece.White ? Down : Up;
        if (move.To == _enPassantAttackSquare)
            this[_enPassantAttackSquare + moveDirectionForOtherColor] = Piece.Empty;
    }

    private Square NewEnPassantSquareWhenMakingMove(Move move, Piece movedPieceType)
    {
        if (movedPieceType != Piece.Pawn)
            return Square.NullSquare;

        if (Math.Abs(move.To - move.From) != 20)
            return Square.NullSquare;

        var moveDirectionForOtherColor = _colorToMove == Piece.White ? Down : Up;
        return move.To + moveDirectionForOtherColor;

    }

    private void HandleCastlingWhenMakingMove(Move move, Piece movedPieceType)
    {
        if (movedPieceType != Piece.King || Math.Abs(move.From-move.To) != 2)
            return;

        var horizontalMoveDelta = move.To - move.From;

        var currentRookPosition = (Square)(horizontalMoveDelta > 0 ? 8 : 1) + 10 * move.From.Rank();
        var newRookPosition = move.From + horizontalMoveDelta / 2;

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
