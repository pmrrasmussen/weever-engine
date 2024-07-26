using Chess.Enums;
using Chess.Structs;

namespace Chess;

public partial class Board
{
    private static readonly Vector[] WhitePawnMoveDirections = [Up, Up + Left, Up + Right, 2 * Up];
    private static readonly Vector[] BlackPawnMoveDirections = WhitePawnMoveDirections.Select(d => -1 * d).ToArray();
    private static readonly Vector[] BishopMoveDirections = [Up + Left, Up + Right, Down + Left, Down + Right];
    private static readonly Vector[] KnightMoveDirections =
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
    private static readonly Vector[] RookMoveDirections = [Up, Left, Right, Down];
    private static readonly Vector[] QueenMoveDirections =
    [
        Up, Left, Right, Down, Up + Left, Up + Right, Down + Left, Down + Right
    ];
    private static readonly Vector[] KingMoveDirections =
    [
        Up, Left, Right, Down, Up + Left, Up + Right, Down + Left, Down + Right
    ];

    private static readonly PieceType[] PromotionPieceTypes =
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

            moves.AddRange(GetPseudoLegalPieceMoves(piece.Value, square));
        }

        return moves;
    }

    private IEnumerable<Move> GetPseudoLegalPieceMoves(Piece piece, Square square)
    {
        var directions = GetMoveVectorsForPiece(piece);


        foreach (var direction in directions)
        {
            var currentPosition = square;

            while ((currentPosition += direction).IsWithinBoard())
            {
                var pieceAtCurrentPosition = this[currentPosition];

                // Stop if we hit our own piece
                if (pieceAtCurrentPosition?.Color == piece.Color)
                    break;

                var moveIsCapture = pieceAtCurrentPosition is not null;

                // TODO: Also check that the move does not cause the king to be in danger!!

                if (piece.Type is PieceType.Pawn)
                {
                    // Diagonal move without capture
                    if (Math.Abs(direction.X) == 1 &&
                        !(currentPosition == _enPassantAttackSquare) &&
                        !moveIsCapture)
                        break;

                    // Two-square moves from non-starting position
                    if (direction.Y == 2 && (square.Y != 1 || this[square+Up] is not null))
                        break;

                    if (direction.Y == -2 && (square.Y != 6 || this[square+Down] is not null))
                        break;

                    // Capturing forward
                    if (direction.Length() == 1 && moveIsCapture)
                        break;

                    // Promotion
                    if (currentPosition.Y % 7 == 0)
                    {
                        foreach (var pieceType in PromotionPieceTypes)
                        {
                            var promoteTo = new Piece(pieceType, piece.Color);
                            yield return new Move(
                                from: square,
                                to: currentPosition,
                                movedPiece: piece,
                                capturedPiece: pieceAtCurrentPosition,
                                promotionTo: promoteTo);
                        }

                        break;
                    }
                }

                yield return new Move(
                    from: square,
                    to: currentPosition,
                    movedPiece: piece,
                    capturedPiece: pieceAtCurrentPosition);

                // Stop if the piece doesn't slide or we performed a capture
                if (!PieceType.SlidingPieces.HasFlag(piece.Type) || moveIsCapture)
                    break;

                // Add castling moves when a rook slides next to the king
                if (piece.Type is PieceType.Rook && currentPosition.Y % 7 == 0)
                {
                    if (currentPosition == Squares.D1 &&
                        piece.Color is Color.White &&
                        _castlingPrivileges.WhiteQueenSide)
                        yield return new Move(Squares.E1, Squares.C1, piece);

                    if (currentPosition == Squares.F1 &&
                        piece.Color is Color.White &&
                        _castlingPrivileges.WhiteKingSide)
                        yield return new Move(Squares.E1, Squares.G1, piece);

                    if (currentPosition == Squares.D8 &&
                        piece.Color is Color.Black &&
                        _castlingPrivileges.BlackQueenSide)
                        yield return new Move(Squares.E8, Squares.C8, piece);

                    if (currentPosition == Squares.F8 &&
                        piece.Color is Color.Black &&
                        _castlingPrivileges.BlackKingSide)
                        yield return new Move(Squares.E8, Squares.G8, piece);
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
