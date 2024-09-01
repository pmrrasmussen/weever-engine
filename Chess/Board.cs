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
    private CastlingPrivileges _castlingPrivileges = CastlingPrivileges.All;
    private Square[] _kingPositions = [ default, default ];

    public Board()
    {
        _pieces = Enumerable.Repeat(Piece.OutOfBounds, 120).ToArray();
        foreach (var square in BoardSquares.All)
        {
            this[square] = Piece.Empty;
        }
    }

    public Piece this[Square square]
    {
        get => _pieces[(int)square];
        set => _pieces[(int)square] = value;
    }

    public bool WhiteToMove => _colorToMove == Piece.White;

    public override string ToString()
    {
        var stringRepresentation = new StringBuilder();
        for (int y = 7; y >= 0; y--)
        {
            for (int x = 0; x < 8; x++)
            {
                var square = (x + 1) + (y + 2) * 10;
                stringRepresentation.Append(_pieces[square].ToBoardString());
            }

            stringRepresentation.Append('\n');
        }

        return stringRepresentation.ToString();
    }

    public bool Equals(Board otherBoard)
    {
        return HistoryAgnosticEquals(otherBoard) && otherBoard._moveHistory.SequenceEqual(_moveHistory);
    }

    public bool HistoryAgnosticEquals(Board otherBoard)
    {
        return BoardSquares.All.All(square => otherBoard[square] == this[square]) &&
               otherBoard._colorToMove == _colorToMove &&
               otherBoard._kingPositions.SequenceEqual(_kingPositions) &&
               otherBoard._castlingPrivileges.Equals(_castlingPrivileges) &&
               otherBoard._enPassantAttackSquare.Equals(_enPassantAttackSquare);
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

        foreach (var square in BoardSquares.All)
                newBoard[square] = this[square];

        return newBoard;
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

    internal void SetKingPositions()
    {
        foreach (var square in BoardSquares.All)
        {
            if ((this[square] & Piece.TypeMask) is not Piece.King)
                continue;

            _kingPositions[this[square].KingPositionIndex()] = square;
        }
    }
}
