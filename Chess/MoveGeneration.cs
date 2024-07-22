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

    private static PieceType[] PromotionPieceTypes =
    [
        PieceType.Bishop,
        PieceType.Knight,
        PieceType.Rook,
        PieceType.Queen
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
        var directions = GetMoveVectorsForPiece(piece);

        var currentPosition = square;

        foreach (var direction in directions)
        {
            while ((currentPosition += direction).IsWithinBoard())
            {
                var pieceAtCurrentPosition = this[currentPosition];
                if (pieceAtCurrentPosition?.Color == piece.Color)
                    break;

                if (piece.Type is PieceType.Pawn)
                {
                    var attemptsCapture = pieceAtCurrentPosition is not null;

                    // Diagonal move without capture
                    if (Math.Abs(direction.Y) == 1 &&
                        !currentPosition.Equals(_enPassantAttackSquare) &&
                        !attemptsCapture)
                        break;

                    // Two-square moves from non-starting position
                    if (direction.X == 2 && square.X != 1)
                        break;

                    if (direction.X == -2 && square.X != 6)
                        break;

                    // Capturing forward
                    if (direction.Size() == 1 && attemptsCapture)
                        break;

                    // Promotion
                    if (currentPosition.Y % 7 == 0)
                    {
                        foreach (var pieceType in PromotionPieceTypes)
                        {
                            var promoteTo = new Piece(pieceType, piece.Color);
                            yield return new Move(square, currentPosition, promoteTo);
                        }

                        break;
                    }
                }

                // TODO: Add king handling

                yield return new Move(square, currentPosition);

                if (!PieceType.SlidingPieces.HasFlag(piece.Type) || pieceAtCurrentPosition is not null)
                    break;

                // Add castling when rook slides next to the king
                if (piece.Type is PieceType.Rook && currentPosition.Y % 7 == 0)
                {
                    if (currentPosition.Equals(Squares.D1) &&
                        piece.Color is Color.White &&
                        _whiteCastlingPrivileges.QueenSideCastling)
                        yield return new Move(Squares.E1, Squares.C1);

                    if (currentPosition.Equals(Squares.F1) &&
                        piece.Color is Color.White &&
                        _whiteCastlingPrivileges.KingSideCastling)
                        yield return new Move(Squares.E1, Squares.G1);

                    if (currentPosition.Equals(Squares.D8) &&
                        piece.Color is Color.White &&
                        _whiteCastlingPrivileges.QueenSideCastling)
                        yield return new Move(Squares.E8, Squares.C8);

                    if (currentPosition.Equals(Squares.F8) &&
                        piece.Color is Color.White &&
                        _whiteCastlingPrivileges.KingSideCastling)
                        yield return new Move(Squares.E8, Squares.G8);
                }
            }
        }
    }

    private IEnumerable<Vector> GetMoveVectorsForPiece(Piece piece)
    {
        return piece.Type switch
        {
            PieceType.King   => KingMoveDirections,
            PieceType.Queen  => QueenMoveDirections,
            PieceType.Rook   => RookMoveDirections,
            PieceType.Bishop => BishopMoveDirections,
            PieceType.Knight => KnightMoveDirections,
            PieceType.Pawn   => piece.Color is Color.White
                ? WhitePawnMoveDirections
                : BlackPawnMoveDirections,
            _ => throw new ArgumentOutOfRangeException($"Unknown piece type {piece.Type}"),
        };
    }
}
