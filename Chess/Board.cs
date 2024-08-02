using System.Text;
using Chess.Enums;
using Chess.Exceptions;
using Chess.Structs;

namespace Chess;

public partial class Board
{
    private static readonly Vector Up = new Vector(0, 1);
    private static readonly Vector Down = new Vector(0, -1);
    private static readonly Vector Left = new Vector(-1, 0);
    private static readonly Vector Right = new Vector(1, 0);

    private readonly Piece?[,] _pieces;
    private Color _colorToMove;
    private Square _enPassantAttackSquare;
    private CastlingPrivileges _castlingPrivileges;
    private Square[] _kingPositions = [ Squares.NullSquare, Squares.NullSquare ];

    public Board()
    {
        _pieces = new Piece?[8, 8];
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
                _pieces[x, y] = null;
        }

        _enPassantAttackSquare = default;
        _castlingPrivileges = new(true, true, true, true);
    }

    public Board Clone()
    {
        var newBoard = new Board();

        newBoard._colorToMove = _colorToMove;
        newBoard._castlingPrivileges = _castlingPrivileges;
        newBoard._kingPositions = _kingPositions.ToArray();
        newBoard._enPassantAttackSquare = _enPassantAttackSquare;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                newBoard._pieces[x, y] = _pieces[x, y];
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
                if (!(otherBoard._pieces[x, y].Equals(_pieces[x, y])))
                    return false;
            }
        }

        return otherBoard._colorToMove == _colorToMove &&
               otherBoard._kingPositions.SequenceEqual(_kingPositions) &&
               otherBoard._castlingPrivileges.Equals(_castlingPrivileges) &&
               otherBoard._enPassantAttackSquare.Equals(_enPassantAttackSquare);
    }

    public Piece? this[Square square]
    {
        get => _pieces[square.X, square.Y];
        set => _pieces[square.X, square.Y] = value;
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

    internal Color ColorToMove
    {
        get => _colorToMove;
        set => _colorToMove = value;
    }

    internal void UpdateKingPositions()
    {
        foreach (var square in Squares.All)
        {
            if (this[square] is not { Type: PieceType.King } piece)
                continue;

            _kingPositions[(int)piece.Color] = square;
        }
    }

    public override string ToString()
    {
        var stringRepresentation = new StringBuilder();
        for (int y = 7; y >= 0; y--)
        {
            for (int x = 0; x < 8; x++)
                stringRepresentation.Append(_pieces[x, y]?.ToString() ?? "\u00B7");

            stringRepresentation.Append('\n');
        }

        return stringRepresentation.ToString();
    }
}
