using Chess.Enums;
using Chess.Exceptions;
using Chess.Structs;

namespace Chess;

public partial class Board
{
    private static readonly Square NullSquare = new (-2, -2);
    private readonly Piece?[,] _pieces;
    private int _moveCount;
    private Color _toMove;
    private Square _enPassantPieceSquare;
    private Square _enPassantAttackSquare;

    public Board()
    {
        _pieces = new Piece?[8, 8];
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
                _pieces[x, y] = null;
        }

        _enPassantPieceSquare = NullSquare;
        _enPassantPieceSquare = NullSquare;
    }

    public void MakeMove(Move move)
    {
        var movedPiece = this[move.From] ?? throw new InvalidMoveException($"There is no piece at {move.From}");

        HandleEnPassant(move, movedPiece);
        HandleCastling(move, movedPiece);

        var pieceToPlace = move.PromotionTo ?? movedPiece;

        this[move.From] = null;
        this[move.To] = pieceToPlace;
        SwitchTurns();
    }

    private Piece? this[Square square]
    {
        get => _pieces[square.X, square.Y];
        set => _pieces[square.X, square.Y] = value;
    }

    private void HandleEnPassant(Move move, Piece movedPiece)
    {
        if (movedPiece.Type is not PieceType.Pawn)
            return;

        // En passant capture
        if (move.To.Equals(_enPassantAttackSquare))
            this[_enPassantPieceSquare] = null;

        // New en passant square
        var verticalMoveDelta = move.To.Y - move.From.Y;
        var enPassantMadePossible = Math.Abs(verticalMoveDelta) == 2;
        _enPassantPieceSquare = enPassantMadePossible ? move.To : NullSquare;
        _enPassantAttackSquare = enPassantMadePossible
            ? new Square(x: move.From.X, y: move.From.Y + verticalMoveDelta / 2)
            : NullSquare;
    }

    private void HandleCastling(Move move, Piece movedPiece)
    {
        if (movedPiece.Type is not PieceType.King)
            return;

        var horizontalMoveDelta = move.To.X - move.From.X;
        var isCastling = Math.Abs(horizontalMoveDelta) == 2;

        if (!isCastling)
            return;

        var currentRookPosition = new Square(
            x: horizontalMoveDelta > 0 ? 7 : 0,
            y: move.From.Y);
        var newRookPosition = new Square(
            move.From.X + horizontalMoveDelta / 2,
            move.From.Y);

        this[newRookPosition] = this[currentRookPosition];
        this[currentRookPosition] = null;
    }

    private void SwitchTurns()
    {
        if (_toMove == Color.Black)
            _moveCount++;

        _toMove = _toMove == Color.White
            ? Color.Black
            : Color.White;
    }

    private Square GetEnPassantAttackSquare()
    {
        if (_enPassantPieceSquare.Equals(NullSquare))
            return NullSquare;

        return _toMove switch
        {
            Color.White => new Square(_enPassantPieceSquare.X, _enPassantPieceSquare.Y + 1),
            Color.Black => new Square(_enPassantPieceSquare.X, _enPassantPieceSquare.Y - 1),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
