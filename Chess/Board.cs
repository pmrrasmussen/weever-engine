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
    private int _halfMoveCount;
    private Color _colorToMove;
    private Square _enPassantAttackSquare;
    private CastlingPrivileges _castlingPrivileges;

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

    public Piece? this[Square square]
    {
        get => _pieces[square.X, square.Y];
        set => _pieces[square.X, square.Y] = value;
    }
}
