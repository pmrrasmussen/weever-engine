using System.Text;
using Chess.Enums;
using Chess.Structs;

namespace Chess;

public partial class Board
{
    private readonly Piece[] _pieces;
    private Stack<BoardMoveDelta> _moveHistory = new();
    private Piece _colorToMove = Piece.White;
    private Square _enPassantAttackSquare;
    private CastlingPrivileges _castlingPrivileges;
    private Square[] _kingPositions = [ Squares.NullSquare, Squares.NullSquare ];

    public Board()
    {
        _pieces = new Piece[64];
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var square = new Square(x, y);
                this[square] = Piece.None;
            }
        }

        _enPassantAttackSquare = default;
        _castlingPrivileges = CastlingPrivileges.All;
    }

    public Board Clone()
    {
        var newBoard = new Board
        {
            _colorToMove = _colorToMove,
            _castlingPrivileges = _castlingPrivileges,
            _kingPositions = _kingPositions.ToArray(),
            _enPassantAttackSquare = _enPassantAttackSquare,
            _moveHistory = new(_moveHistory.Reverse())
        };

        for (var x = 0; x < 8; x++)
        {
            for (var y = 0; y < 8; y++)
            {
                var square = new Square(x, y);
                newBoard[square] = this[square];
            }
        }

        return newBoard;
    }

    public bool Equals(Board otherBoard)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var square = new Square(x, y);
                if (!(otherBoard[square].Equals(this[square])))
                    return false;
            }
        }

        return otherBoard._colorToMove == _colorToMove &&
               otherBoard._kingPositions.SequenceEqual(_kingPositions) &&
               otherBoard._castlingPrivileges.Equals(_castlingPrivileges) &&
               otherBoard._enPassantAttackSquare.Equals(_enPassantAttackSquare) &&
               otherBoard._moveHistory.SequenceEqual(_moveHistory);
    }

    public bool HistoryAgnosticEquals(Board otherBoard)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var square = new Square(x, y);
                if (!(otherBoard[square].Equals(this[square])))
                    return false;
            }
        }

        return otherBoard._colorToMove == _colorToMove &&
               otherBoard._kingPositions.SequenceEqual(_kingPositions) &&
               otherBoard._castlingPrivileges.Equals(_castlingPrivileges) &&
               otherBoard._enPassantAttackSquare.Equals(_enPassantAttackSquare);
    }

    public Piece this[Square square]
    {
        get => _pieces[square.X + square.Y * 8];
        set => _pieces[square.X + square.Y * 8] = value;
    }

    internal CastlingPrivileges CastlingPrivileges
    {
        get => _castlingPrivileges;
        set => _castlingPrivileges = value;
    }

    internal Square EnPassantAttackSquare
    {
        get => _enPassantAttackSquare;
        set => _enPassantAttackSquare = value;
    }

    internal Piece ColorToMove
    {
        get => _colorToMove;
        set => _colorToMove = value;
    }

    internal void UpdateKingPositions()
    {
        foreach (var square in Squares.All)
        {
            if (!this[square].HasFlag(Piece.King))
                continue;

            var kingIndex = this[square].HasFlag(Piece.White) ? 0 : 1;
            _kingPositions[kingIndex] = square;
        }
    }

    public override string ToString()
    {
        var stringRepresentation = new StringBuilder();
        for (int y = 7; y >= 0; y--)
        {
            for (int x = 0; x < 8; x++)
            {
                var square = new Square(x, y);
                stringRepresentation.Append(PieceExtensions.PieceToString(this[square]));
            }

            stringRepresentation.Append('\n');
        }

        return stringRepresentation.ToString();
    }
}
