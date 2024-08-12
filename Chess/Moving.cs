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

        HandleEnPassantCaptureWhenMakingMove(move, movedPiece);
        HandleCastlingWhenMakingMove(move, movedPiece);
        SetCastlingPrivilegesWhenMakingMove(move, movedPiece);
        _enPassantAttackSquare = NewEnPassantSquareWhenMakingMove(move, movedPiece);

        var pieceToPlace = move.PromotionTo !=  Piece.None ? move.PromotionTo : movedPiece;
        this[move.From] = Piece.None;
        this[move.To] = pieceToPlace;

        if (pieceToPlace.HasFlag(Piece.King))
        {
            var kingIndex = _colorToMove == Piece.White ? 0 : 1;
            _kingPositions[kingIndex] = move.To;
        }

        _colorToMove = _colorToMove == Piece.White ? Piece.Black : Piece.White;
    }

    public void UndoLastMove()
    {
        var moveDelta = _moveHistory.Pop();

        var movedPiece = this[moveDelta.Move.To];

        _castlingPrivileges = moveDelta.CastlingPrivileges;
        _enPassantAttackSquare = moveDelta.EnPassantAttackSquare;
        _colorToMove = _colorToMove == Piece.White ? Piece.Black : Piece.White;

        var lastMove = moveDelta.Move;

        HandleUndoEnPassant(lastMove, movedPiece);
        HandleUndoCastling(lastMove, movedPiece);

        this[lastMove.From] = moveDelta.Move.PromotionTo == Piece.None
            ? movedPiece
            : Piece.Pawn | _colorToMove;
        this[lastMove.To] = moveDelta.DirectlyCapturedPiece;

        if (movedPiece.HasFlag(Piece.King))
        {
            var kingIndex = _colorToMove == Piece.White ? 0 : 1;
            _kingPositions[kingIndex] = lastMove.From;
        }
    }

    private void HandleUndoEnPassant(Move move, Piece movedPiece)
    {
        if (!movedPiece.HasFlag(Piece.Pawn) || _enPassantAttackSquare == default)
            return;

        var moveDirectionForOtherColor = _colorToMove == Piece.White ? Down : Up;

        if (move.To == _enPassantAttackSquare)
        {
            var otherColor = _colorToMove == Piece.White ? Piece.Black : Piece.White;
            this[_enPassantAttackSquare + moveDirectionForOtherColor] = Piece.Pawn | otherColor;
        }

    }

    private void HandleUndoCastling(Move move, Piece movedPiece)
    {
        if (!movedPiece.HasFlag(Piece.King) || Math.Abs((move.From-move.To).X) != 2)
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

    private void HandleEnPassantCaptureWhenMakingMove(Move move, Piece movedPiece)
    {
        if (!movedPiece.HasFlag(Piece.Pawn) || _enPassantAttackSquare == default)
            return;

        var moveDirectionForOtherColor = _colorToMove == Piece.White ? Down : Up;
        if (move.To == _enPassantAttackSquare)
            this[_enPassantAttackSquare + moveDirectionForOtherColor] = Piece.None;
    }

    private Square NewEnPassantSquareWhenMakingMove(Move move, Piece movedPiece)
    {
        if (!movedPiece.HasFlag(Piece.Pawn))
            return default;

        var verticalMoveDelta = move.To.Y - move.From.Y;
        if (Math.Abs(verticalMoveDelta) != 2)
            return default;

        var moveDirectionForOtherColor = _colorToMove == Piece.White ? Down : Up;
        return move.To + moveDirectionForOtherColor;

    }

    private void HandleCastlingWhenMakingMove(Move move, Piece movedPiece)
    {
        if (!movedPiece.HasFlag(Piece.King) || Math.Abs((move.From-move.To).X) != 2)
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

    private void SetCastlingPrivilegesWhenMakingMove(Move move, Piece movedPiece)
    {
        var (whiteKingSide, whiteQueenSide, blackKingSide, blackQueenSide) =
            (
                _castlingPrivileges.WhiteKingSide,
                _castlingPrivileges.WhiteQueenSide,
                _castlingPrivileges.BlackKingSide,
                _castlingPrivileges.BlackQueenSide
            );

        if (movedPiece.HasFlag(Piece.King))
        {
            if (_colorToMove == Piece.White)
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
