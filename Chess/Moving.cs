using Chess.Enums;
using Chess.Structs;

namespace Chess;

public partial class Board
{
    public void MakeMove(Move move)
    {
        _moveHistory.Push(new BoardMoveDelta(move, _castlingPrivileges, _enPassantAttackSquare));

        var movedPiece = this[move.From] ?? throw new ArgumentOutOfRangeException(
            $"Invalid move {move} with piece {move.MovedPiece} on board \n{ToString()}");

        HandleEnPassantWhenMakingMove(move);
        HandleCastlingWhenMakingMove(move);
        SetCastlingPrivilegesWhenMakingMove(move);

        var pieceToPlace = move.PromotionTo ?? move.MovedPiece;
        this[move.From] = null;
        this[move.To] = pieceToPlace;

        if (pieceToPlace.Type is PieceType.King)
            _kingPositions[(int)_colorToMove] = move.To;

        _colorToMove = _colorToMove.Flip();
    }

    public void UndoLastMove()
    {
        var moveDelta = _moveHistory.Pop();

        _castlingPrivileges = moveDelta.CastlingPrivileges;
        _enPassantAttackSquare = moveDelta.EnPassantAttackSquare;
        _colorToMove = _colorToMove.Flip();

        var lastMove = moveDelta.Move;

        HandleUndoEnPassant(lastMove);
        HandleUndoCastling(lastMove);

        this[lastMove.From] = lastMove.MovedPiece;
        this[lastMove.To] = lastMove.CapturedPiece;

        if (lastMove.MovedPiece.Type is PieceType.King)
            _kingPositions[(int)_colorToMove] = lastMove.From;
    }

    private void HandleUndoEnPassant(Move move)
    {
        if (move.MovedPiece.Type is not PieceType.Pawn || _enPassantAttackSquare == default)
            return;

        var moveDirectionForOtherColor = _colorToMove is Color.White ? Down : Up;

        if (move.To == _enPassantAttackSquare)
        {
            this[_enPassantAttackSquare + moveDirectionForOtherColor] = new Piece(
                type: PieceType.Pawn,
                color: _colorToMove.Flip());
        }

    }

    private void HandleUndoCastling(Move move)
    {
        if (move.MovedPiece.Type is not PieceType.King || Math.Abs((move.From-move.To).X) != 2)
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

    private void HandleEnPassantWhenMakingMove(Move move)
    {
        if (move.MovedPiece.Type is not PieceType.Pawn || _enPassantAttackSquare == default)
            return;

        var moveDirectionForOtherColor = _colorToMove is Color.White ? Down : Up;

        // En passant capture
        if (move.To == _enPassantAttackSquare)
            this[_enPassantAttackSquare + moveDirectionForOtherColor] = null;

        // New en passant square
        var verticalMoveDelta = move.To.Y - move.From.Y;
        var enPassantMadePossible = Math.Abs(verticalMoveDelta) == 2;
        _enPassantAttackSquare = enPassantMadePossible
            ? move.To + moveDirectionForOtherColor
            : default;
    }

    private void HandleCastlingWhenMakingMove(Move move)
    {
        if (move.MovedPiece.Type is not PieceType.King || Math.Abs((move.From-move.To).X) != 2)
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

    private void SetCastlingPrivilegesWhenMakingMove(Move move)
    {
        var (whiteKingSide, whiteQueenSide, blackKingSide, blackQueenSide) =
            (
                _castlingPrivileges.WhiteKingSide,
                _castlingPrivileges.WhiteQueenSide,
                _castlingPrivileges.BlackKingSide,
                _castlingPrivileges.BlackQueenSide
            );

        if (move.MovedPiece.Type is PieceType.King)
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
