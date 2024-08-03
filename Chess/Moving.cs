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

        var movedPiece = this[move.From] ?? throw new ArgumentOutOfRangeException(
            $"Invalid move {move} on board \n{ToString()}");

        HandleEnPassantCaptureWhenMakingMove(move, movedPiece);
        HandleCastlingWhenMakingMove(move, movedPiece);
        SetCastlingPrivilegesWhenMakingMove(move, movedPiece);
        _enPassantAttackSquare = NewEnPassantSquareWhenMakingMove(move, movedPiece);

        var pieceToPlace = move.PromotionTo ?? movedPiece;
        this[move.From] = null;
        this[move.To] = pieceToPlace;

        if (pieceToPlace.Type is PieceType.King)
            _kingPositions[(int)_colorToMove] = move.To;

        _colorToMove = _colorToMove.Flip();
    }

    public void UndoLastMove()
    {
        var moveDelta = _moveHistory.Pop();

        var movedPiece = this[moveDelta.Move.To] ?? throw new ArgumentOutOfRangeException(
            $"Invalid undo of move {moveDelta.Move} on board \n{ToString()}");

        _castlingPrivileges = moveDelta.CastlingPrivileges;
        _enPassantAttackSquare = moveDelta.EnPassantAttackSquare;
        _colorToMove = _colorToMove.Flip();

        var lastMove = moveDelta.Move;

        HandleUndoEnPassant(lastMove, movedPiece);
        HandleUndoCastling(lastMove, movedPiece);

        this[lastMove.From] = moveDelta.Move.PromotionTo is null
            ? movedPiece
            : new Piece(PieceType.Pawn, _colorToMove);
        this[lastMove.To] = moveDelta.DirectlyCapturedPiece;

        if (movedPiece.Type is PieceType.King)
            _kingPositions[(int)_colorToMove] = lastMove.From;
    }

    private void HandleUndoEnPassant(Move move, Piece movedPiece)
    {
        if (movedPiece.Type is not PieceType.Pawn || _enPassantAttackSquare == default)
            return;

        var moveDirectionForOtherColor = _colorToMove is Color.White ? Down : Up;

        if (move.To == _enPassantAttackSquare)
        {
            this[_enPassantAttackSquare + moveDirectionForOtherColor] = new Piece(
                type: PieceType.Pawn,
                color: _colorToMove.Flip());
        }

    }

    private void HandleUndoCastling(Move move, Piece movedPiece)
    {
        if (movedPiece.Type is not PieceType.King || Math.Abs((move.From-move.To).X) != 2)
            return;

        var horizontalMoveDelta = move.To.X - move.From.X;

        var originalRookPosition = new Square(
            x: horizontalMoveDelta > 0 ? 7 : 0,
            y: move.From.Y);
        var currentRookPosition = new Square(
            move.From.X + horizontalMoveDelta / 2,
            move.From.Y);

        this[originalRookPosition] = this[currentRookPosition];
        this[currentRookPosition] = null;
    }

    private void HandleEnPassantCaptureWhenMakingMove(Move move, Piece movedPiece)
    {
        if (movedPiece.Type is not PieceType.Pawn || _enPassantAttackSquare == default)
            return;

        var moveDirectionForOtherColor = _colorToMove is Color.White ? Down : Up;
        if (move.To == _enPassantAttackSquare)
            this[_enPassantAttackSquare + moveDirectionForOtherColor] = null;
    }

    private Square NewEnPassantSquareWhenMakingMove(Move move, Piece movedPiece)
    {
        if (movedPiece.Type is not PieceType.Pawn)
            return default;

        var verticalMoveDelta = move.To.Y - move.From.Y;
        if (Math.Abs(verticalMoveDelta) != 2)
            return default;

        var moveDirectionForOtherColor = _colorToMove is Color.White ? Down : Up;
        return move.To + moveDirectionForOtherColor;

    }

    private void HandleCastlingWhenMakingMove(Move move, Piece movedPiece)
    {
        if (movedPiece.Type is not PieceType.King || Math.Abs((move.From-move.To).X) != 2)
            return;

        var horizontalMoveDelta = move.To.X - move.From.X;

        var currentRookPosition = new Square(
            x: horizontalMoveDelta > 0 ? 7 : 0,
            y: move.From.Y);
        var newRookPosition = new Square(
            move.From.X + horizontalMoveDelta / 2,
            move.From.Y);

        this[newRookPosition] = this[currentRookPosition];
        this[currentRookPosition] = null;
    }

    private void SetCastlingPrivilegesWhenMakingMove(Move move, Piece movedPiece)
    {
        var (whiteKingSide, whiteQueenSide, blackKingSide, blackQueenSide) =
            (
                _castlingPrivileges.WhiteKingSide,
                _castlingPrivileges.WhiteQueenSide,
                _castlingPrivileges.BlackKingSide,
                _castlingPrivileges.BlackQueenSide
            );

        if (movedPiece.Type is PieceType.King)
        {
            if (_colorToMove is Color.White)
                (whiteKingSide, whiteQueenSide) = (false, false);
            else
                (blackKingSide, blackQueenSide) = (false, false);
        }

        whiteQueenSide &= move.From != Squares.A1 && move.To != Squares.A1;
        whiteKingSide &= move.From != Squares.H1 && move.To != Squares.H1;
        blackQueenSide &= move.From != Squares.A8 && move.To != Squares.A8;
        blackKingSide &= move.From != Squares.H8 && move.To != Squares.H8;

        _castlingPrivileges = new (whiteKingSide, whiteQueenSide, blackKingSide, blackQueenSide);
    }
}
