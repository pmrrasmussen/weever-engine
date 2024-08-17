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

        var pieceToPlace = move.PromotionTo !=  Piece.None ? move.PromotionTo : movedPiece;
        this[move.From] = Piece.None;
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

        this[lastMove.From] = moveDelta.Move.PromotionTo == Piece.None
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
        if (movedPieceType != Piece.Pawn || _enPassantAttackSquare == Squares.NullSquare)
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
            Math.Abs((move.From-move.To).X) != 2)
            return;

        var horizontalMoveDelta = move.To.X - move.From.X;

        var originalRookPosition = new Square(
            x: horizontalMoveDelta > 0 ? 7 : 0,
            y: move.From.Y);
        var currentRookPosition = new Square(
            move.From.X + horizontalMoveDelta / 2,
            move.From.Y);

        this[originalRookPosition] = this[currentRookPosition];
        this[currentRookPosition] = Piece.None;
    }

    private void HandleEnPassantCaptureWhenMakingMove(Move move, Piece movedPieceType)
    {
        if (movedPieceType != Piece.Pawn || _enPassantAttackSquare == Squares.NullSquare)
            return;

        var moveDirectionForOtherColor = _colorToMove == Piece.White ? Down : Up;
        if (move.To == _enPassantAttackSquare)
            this[_enPassantAttackSquare + moveDirectionForOtherColor] = Piece.None;
    }

    private Square NewEnPassantSquareWhenMakingMove(Move move, Piece movedPieceType)
    {
        if (movedPieceType != Piece.Pawn)
            return Squares.NullSquare;

        var verticalMoveDelta = move.To.Y - move.From.Y;
        if (Math.Abs(verticalMoveDelta) != 2)
            return Squares.NullSquare;

        var moveDirectionForOtherColor = _colorToMove == Piece.White ? Down : Up;
        return move.To + moveDirectionForOtherColor;

    }

    private void HandleCastlingWhenMakingMove(Move move, Piece movedPieceType)
    {
        if (movedPieceType != Piece.King || Math.Abs((move.From-move.To).X) != 2)
            return;

        var horizontalMoveDelta = move.To.X - move.From.X;

        var currentRookPosition = new Square(
            x: horizontalMoveDelta > 0 ? 7 : 0,
            y: move.From.Y);
        var newRookPosition = new Square(
            move.From.X + horizontalMoveDelta / 2,
            move.From.Y);

        this[newRookPosition] = this[currentRookPosition];
        this[currentRookPosition] = Piece.None;
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

        if (move.From == Squares.A1 || move.To == Squares.A1)
            _castlingPrivileges &= CastlingPrivileges.Black | CastlingPrivileges.WhiteKingSide;

        if (move.From == Squares.H1 || move.To == Squares.H1)
            _castlingPrivileges &= CastlingPrivileges.Black | CastlingPrivileges.WhiteQueenSide;

        if (move.From == Squares.A8 || move.To == Squares.A8)
            _castlingPrivileges &= CastlingPrivileges.White | CastlingPrivileges.BlackKingSide;

        if (move.From == Squares.H8 || move.To == Squares.H8)
            _castlingPrivileges &= CastlingPrivileges.White | CastlingPrivileges.BlackQueenSide;
    }
}
