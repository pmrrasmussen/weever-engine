using Chess.Enums;

namespace Chess;

public partial class Board
{
    private long _hash;

    private long[][] _hashTables;

    public long ZobristHash => _hash;

    private void FillHashTables()
    {
        return;
    }

    private void UpdateHashOnReplacingPieceOnSquare(Square square, Piece newPiece)
    {
        if (this[square] != Piece.Empty)
        {
            _hash ^= _hashTables[(int)square][(int)this[square]];
        }

        _hash ^= _hashTables[(int)square][(int)newPiece];
    }

    private void RecomputeHash()
    {
        foreach (var square in Squares.All)
        {
            if (this[square] != Piece.Empty)
            {
                _hash ^= _hashTables[(int)square][(int)this[square]];
            }
        }
    }
}
