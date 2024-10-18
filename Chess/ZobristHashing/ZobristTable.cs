using Chess.Enums;

namespace Chess.ZobristHashing;

public class ZobristTable
{
    private const int BoardSize = 120;
    private static Random _random = new Random();
    private readonly long[][] _table;

    public ZobristTable()
    {
        _table =
        [
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            GetPieceTable(),
            GetPieceTable(),
            GetPieceTable(),
            GetPieceTable(),
            GetPieceTable(),
            GetPieceTable(),
            [],
            [],
            GetPieceTable(),
            GetPieceTable(),
            GetPieceTable(),
            GetPieceTable(),
            GetPieceTable(),
            GetPieceTable(),
        ];
    }

    public long this[Piece piece, Square square] => _table[(int)piece][(int)square];

    private static long[] GetPieceTable()
    {
        var pieceTable = new long[BoardSize];
        for (int i = 0; i < BoardSize; i++)
        {
            pieceTable[i] = _random.NextInt64();
        }
        return pieceTable;
    }
}
