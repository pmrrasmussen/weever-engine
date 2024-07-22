using Chess.Enums;
using Chess.Structs;

namespace Chess;

public partial class Board
{
    private static Vector Up = new Vector(0, 1);
    private static Vector Down = new Vector(0, -1);
    private static Vector Left = new Vector(-1, 0);
    private static Vector Right = new Vector(1, 0);

    private static Vector[] WhitePawnMoveDirections = [Up, Up + Left, Up + Right, 2 * Up];
    private static Vector[] BlackPawnMoveDirections = WhitePawnMoveDirections.Select(d => -1 * d).ToArray();
    private static Vector[] BishopMoveDirections = [Up + Left, Up + Right, Down + Left, Down + Right];
    private static Vector[] KnightMoveDirections =
    [
        Up + 2 * Left,
        2 * Up + Left,
        2 * Up + Right,
        Up + 2 * Right,
        Down + 2 * Right,
        2 * Down + Right,
        2 * Down + Left,
        Down + 2 * Left,
    ];
    private static Vector[] RookMoveDirections = [Up, Left, Right, Down];
    private static Vector[] QueenMoveDirections =
    [
        Up, Left, Right, Down, Up + Left, Up + Right, Down + Left, Down + Right
    ];
    private static Vector[] KingMoveDirections =
    [
        Up, Left, Right, Down, Up + Left, Up + Right, Down + Left, Down + Right
    ];

    public IEnumerable<Move> GetLegalMoves()
    {
        var moves = new List<Move>();

        foreach (var square in Squares.All)
        {
            var piece = this[square];
            if (piece is null)
                continue;

            moves.AddRange(GetLegalPieceMoves(piece.Value, square));
        }

        return moves;
    }

    private IEnumerable<Move> GetLegalPieceMoves(Piece piece, Square square)
    {
        var moves = new List<Move>();
        var directions = GetMoveVectorsForPiece(piece);

        var currentPosition = square;

        foreach (var direction in directions)
        {
            currentPosition += direction;
            while (currentPosition.IsWithinBoard())
            {
                var pieceAtCurrentPosition = this[currentPosition];
                if (pieceAtCurrentPosition?.Color == piece.Color)
                    break;

                moves.Add(new Move(square, currentPosition));

                if (!PieceType.SlidingPieces.HasFlag(piece.Type) || pieceAtCurrentPosition is not null)
                    break;

                currentPosition += direction;
            }
        }

        return moves;
    }

    private IEnumerable<Vector> GetMoveVectorsForPiece(Piece piece)
    {
        return piece.Type switch
        {
            PieceType.King => KingMoveDirections,
            PieceType.Queen => QueenMoveDirections,
            PieceType.Rook => RookMoveDirections,
            PieceType.Bishop => BishopMoveDirections,
            PieceType.Knight => KnightMoveDirections,
            PieceType.Pawn => piece.Color is Color.White
                ? WhitePawnMoveDirections
                : BlackPawnMoveDirections,
            _ => throw new ArgumentOutOfRangeException($"Unknown piece type {piece.Type}"),
        };
    }
}
