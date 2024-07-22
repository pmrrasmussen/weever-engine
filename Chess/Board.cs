using Chess.Enums;
using Chess.Exceptions;
using Chess.Structs;

namespace Chess;

public partial class Board
{
    private static readonly Square NullSquare = new (int.MaxValue, int.MaxValue);
    private readonly Piece?[,] _pieces;
    private int _moveCount;
    private Color _colorToMove;
    private Square _enPassantPieceSquare;
    private Square _enPassantAttackSquare;
    private (bool KingSideCastling, bool QueenSideCastling) _whiteCastlingPrivileges;
    private (bool KingSideCastling, bool QueenSideCastling) _blackCastlingPrivileges;

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

        _whiteCastlingPrivileges = (true, true);
        _blackCastlingPrivileges = (true, true);
    }

    public void MakeMove(Move move)
    {
        var movedPiece = this[move.From]
            ?? throw new InvalidMoveException($"There is no piece at {move.From}");

        HandleEnPassant(move, movedPiece);
        HandleCastling(move, movedPiece);
        SetCastlingPrivileges(move, movedPiece);

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
        (_enPassantPieceSquare, _enPassantAttackSquare) = enPassantMadePossible
            ? (move.To, new Square(x: move.From.X, y: move.From.Y + verticalMoveDelta / 2))
            : (NullSquare, NullSquare);
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

    private void SetCastlingPrivileges(Move move, Piece movedPiece)
    {
        if (movedPiece.Type is PieceType.King)
        {
            if (_colorToMove is Color.White)
                _whiteCastlingPrivileges = (false, false);
            else
                _blackCastlingPrivileges = (false, false);
        }

        if (move.From.Equals(Squares.A1) || move.To.Equals(Squares.A1))
            _whiteCastlingPrivileges.QueenSideCastling = false;
        if (move.From.Equals(Squares.H1) || move.To.Equals(Squares.H1))
            _whiteCastlingPrivileges.KingSideCastling = false;
        if (move.From.Equals(Squares.A8) || move.To.Equals(Squares.A8))
            _blackCastlingPrivileges.QueenSideCastling = false;
        if (move.From.Equals(Squares.H8) || move.To.Equals(Squares.H8))
            _blackCastlingPrivileges.KingSideCastling = false;
    }

    private void SwitchTurns()
    {
        if (_colorToMove == Color.Black)
            _moveCount++;

        _colorToMove = _colorToMove == Color.White
            ? Color.Black
            : Color.White;
    }
}
