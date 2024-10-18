#define Hashing

using Chess.Enums;
using Chess.ZobristHashing;

namespace Chess;

public partial class Board
{
    private long _hash;

    private readonly ZobristTable _hashTable = new ();

    public long ZobristHash => _hash;

    private void UpdateHashOnReplacingPieceOnSquare(Square square, Piece newPiece)
    {
#if Hashing
        if (this[square] != Piece.Empty)
            _hash ^= _hashTable[this[square], square];

        if (newPiece != Piece.Empty)
            _hash ^= _hashTable[newPiece, square];
#endif
    }

    private void RecomputeHash()
    {
        foreach (var square in BoardSquares.All)
        {
            if (this[square] != Piece.Empty)
            {
                _hash ^= _hashTable[this[square], square];
            }
        }
    }
}
