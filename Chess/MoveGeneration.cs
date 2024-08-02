using System.Diagnostics;
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
    private static readonly Vector[] AllMoveDirections = QueenMoveDirections.Concat(KnightMoveDirections).ToArray();

    private static readonly PieceType[] PromotionPieceTypes =
    [
        PieceType.Bishop,
        PieceType.Knight,
        PieceType.Rook,
        PieceType.Queen
    ];

    private bool IsCheck(Color color)
    {
        var kingPosition = _kingPositions[(int)color];

        var forwardsDirection = color is Color.White ? Up : Down;

        Square[] potentialPawnAttackSquares =
        [
            kingPosition + forwardsDirection + Right,
            kingPosition + forwardsDirection + Left,
        ];

        foreach (var pawnAttackSquare in potentialPawnAttackSquares)
        {
            if (!pawnAttackSquare.IsWithinBoard())
                continue;

            if (this[pawnAttackSquare] is Piece potentialPawn &&
                potentialPawn.Color != color &&
                potentialPawn.Type is PieceType.Pawn)
                return true;
        }

        foreach (var moveDirection in AllMoveDirections)
        {
            var directionLength = moveDirection.Length();

            var currentPosition = kingPosition;
            while ((currentPosition += moveDirection).IsWithinBoard())
            {
                if (this[currentPosition] is not Piece piece)
                {
                    if (directionLength == 3)
                        break;

                    continue;
                }

                if (piece.Color == color)
                    break;

                switch (directionLength)
                {
                    case 1 when piece.Type is PieceType.Queen or PieceType.Rook:
                    case 2 when piece.Type is PieceType.Queen or PieceType.Bishop:
                    case 3 when piece.Type is PieceType.Knight:
                        return true;
                }

                break;
            }
        }

        return false;
    }

    public List<Move> GetLegalMoves()
    {
        var moves = new List<Move>();

        foreach (var square in Squares.All)
        {
            var potentialPiece = this[square];
            if (potentialPiece is not { } piece || piece.Color != _colorToMove)
                continue;

            var currentColor = _colorToMove;
            foreach (var move in GetPseudoLegalPieceMoves(piece, square))
            {
                var movedPiece = this[move.From] ?? throw new ArgumentOutOfRangeException(
                    $"Invalid move {move} on board \n{ToString()}");

                // Handle castling through check
                if (movedPiece.Type is PieceType.King && Math.Abs((move.To - move.From).X) == 2 )
                {
                    if (IsCheck(currentColor))
                        continue;

                    var inBetweenSquare = (move.To - move.From).X < 0
                        ? move.To + Right
                        : move.To + Left;
                    var inBetweenMove = new Move(
                        from: move.From,
                        to: inBetweenSquare,
                        movedPiece: movedPiece);

                    MakeMove(inBetweenMove);
                    var isCheck = IsCheck(currentColor);
                    UndoLastMove();
                    if (isCheck)
                        continue;
                }

                MakeMove(move);
                if (!IsCheck(currentColor))
                    moves.Add(move);
                UndoLastMove();
            }
        }

        return moves;
    }

    private List<Move> GetPseudoLegalPieceMoves(Piece piece, Square square)
    {
        var moves = new List<Move>();

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
                    if (direction.Y == 2 && (square.Y != 1 || this[square+Up] is not null || moveIsCapture))
                        break;

                    if (direction.Y == -2 && (square.Y != 6 || this[square+Down] is not null || moveIsCapture))
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
                            moves.Add(new Move(
                                from: square,
                                to: currentPosition,
                                movedPiece: piece,
                                capturedPiece: pieceAtCurrentPosition,
                                promotionTo: promoteTo));
                        }

                        break;
                    }
                }

                moves.Add(new Move(
                    from: square,
                    to: currentPosition,
                    movedPiece: piece,
                    capturedPiece: pieceAtCurrentPosition));

                // Stop if the piece doesn't slide or we performed a capture
                if (!PieceType.SlidingPieces.HasFlag(piece.Type) || moveIsCapture)
                    break;

                // Add castling moves when a rook slides next to the king
                if (piece.Type is not PieceType.Rook || currentPosition.Y % 7 != 0)
                    continue;

                if (currentPosition == Squares.D1 &&
                    piece.Color is Color.White &&
                    _castlingPrivileges.WhiteQueenSide)
                    moves.Add(new Move(Squares.E1, Squares.C1, new Piece(PieceType.King, Color.White)));

                if (currentPosition == Squares.F1 &&
                    piece.Color is Color.White &&
                    _castlingPrivileges.WhiteKingSide)
                    moves.Add(new Move(Squares.E1, Squares.G1, new Piece(PieceType.King, Color.White)));

                if (currentPosition == Squares.D8 &&
                    piece.Color is Color.Black &&
                    _castlingPrivileges.BlackQueenSide)
                    moves.Add(new Move(Squares.E8, Squares.C8, new Piece(PieceType.King, Color.Black)));

                if (currentPosition == Squares.F8 &&
                    piece.Color is Color.Black &&
                    _castlingPrivileges.BlackKingSide)
                    moves.Add(new Move(Squares.E8, Squares.G8, new Piece(PieceType.King, Color.Black)));
            }
        }

        return moves;
    }

    private Vector[] GetMoveVectorsForPiece(Piece piece)
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
