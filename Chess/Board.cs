using System.Text;
using Chess.Enums;
using Chess.Structs;

namespace Chess;

public partial class Board
{
    private readonly Piece[] _pieces = Enumerable.Repeat(Piece.None, 64).ToArray();
    private Stack<BoardMoveDelta> _moveHistory = new();
    private Piece _colorToMove = Piece.White;
    private Square _enPassantAttackSquare = Squares.NullSquare;
    private CastlingPrivileges _castlingPrivileges = CastlingPrivileges.All;
    private Square[] _kingPositions = [ Squares.NullSquare, Squares.NullSquare ];

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

        foreach (var square in Squares.All)
                newBoard[square] = this[square];

        return newBoard;
    }

    public bool Equals(Board otherBoard)
    {
        foreach (var square in Squares.All)
            if (!(otherBoard[square].Equals(this[square])))
                return false;

        return otherBoard._colorToMove == _colorToMove &&
               otherBoard._kingPositions.SequenceEqual(_kingPositions) &&
               otherBoard._castlingPrivileges.Equals(_castlingPrivileges) &&
               otherBoard._enPassantAttackSquare.Equals(_enPassantAttackSquare) &&
               otherBoard._moveHistory.SequenceEqual(_moveHistory);
    }

    public bool HistoryAgnosticEquals(Board otherBoard)
    {
        foreach (var square in Squares.All)
                if (!(otherBoard[square].Equals(this[square])))
                    return false;

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
            if ((this[square] & Piece.TypeMask) is not Piece.King)
                continue;

            _kingPositions[this[square].KingPositionIndex()] = square;
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
                stringRepresentation.Append(this[square].AsString());
            }

            stringRepresentation.Append('\n');
        }

        return stringRepresentation.ToString();
    }
}
