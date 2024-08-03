using System.Drawing;
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

    private static readonly Piece[] PromotionPieceTypes =
    [
        Piece.Bishop,
        Piece.Knight,
        Piece.Rook,
        Piece.Queen
    ];

    private bool IsCheck(Piece color)
    {
        var kingIndex = color is Piece.White ? 0 : 1;
        var kingPosition = _kingPositions[kingIndex];

        var forwardsDirection = color.HasFlag(Piece.White) ? Up : Down;

        if (IsOppositeColouredPawn(color, kingPosition + forwardsDirection + Right) ||
            IsOppositeColouredPawn(color, kingPosition + forwardsDirection + Left))
            return true;

        foreach (var moveDirection in AllMoveDirections)
        {
            var directionLength = moveDirection.Length();

            var currentPosition = kingPosition;
            while ((currentPosition += moveDirection).IsWithinBoard())
            {
                var piece = this[currentPosition];
                if (piece is Piece.None)
                {
                    if (directionLength == 3)
                        break;

                    continue;
                }

                if (piece.HasFlag(color))
                    break;

                switch (directionLength)
                {
                    case 1 when (piece & (Piece.Queen | Piece.Rook)) != 0:
                    case 2 when (piece & (Piece.Queen | Piece.Bishop)) != 0:
                    case 3 when piece.HasFlag(Piece.Knight):
                        return true;
                }

                break;
            }
        }

        return false;
    }

    private bool IsOppositeColouredPawn(Piece color, Square square)
    {
        if (!square.IsWithinBoard())
            return false;

        var piece = this[square];
        return piece != Piece.None &&
               !piece.HasFlag(color) &&
               piece.HasFlag(Piece.Pawn);
    }

    public List<Move> GetLegalMoves()
    {
        var pseudoLegalMoves = new List<Move>();

        foreach (var square in Squares.All)
        {
            var piece = this[square];
            if (piece == Piece.None || !piece.HasFlag(_colorToMove))
                continue;

            AddPseudoLegalPieceMoves(piece, square, pseudoLegalMoves);
        }

        var moves = new List<Move>(pseudoLegalMoves.Count);

        foreach (var move in pseudoLegalMoves)
        {
            if (!MovesIntoCheck(move))
            {
                moves.Add(move);
            }
        }

        return moves;
    }

    private bool MovesIntoCheck(Move move)
    {
        bool isCheck;

        var currentColor = _colorToMove;
        var movedPiece = this[move.From];

        // Handle castling through check
        if (movedPiece.HasFlag(Piece.King) && Math.Abs((move.To - move.From).X) == 2)
        {
            if (IsCheck(currentColor))
                return true;

            var inBetweenSquare = (move.To - move.From).X < 0
                ? move.To + Right
                : move.To + Left;
            var inBetweenMove = new Move(
                from: move.From,
                to: inBetweenSquare);

            MakeMove(inBetweenMove);
            isCheck = IsCheck(currentColor);
            UndoLastMove();
            if (isCheck)
                return true;
        }

        MakeMove(move);
        isCheck = IsCheck(currentColor);
        UndoLastMove();

        return isCheck;
    }

    private void AddPseudoLegalPieceMoves(Piece piece, Square square, List<Move> moves)
    {
        var directions = GetMoveVectorsForPiece(piece);

        foreach (var direction in directions)
        {
            var currentPosition = square;

            while ((currentPosition += direction).IsWithinBoard())
            {
                var pieceAtCurrentPosition = this[currentPosition];

                // Stop if we hit our own piece
                if (pieceAtCurrentPosition is not Piece.None &&
                    (byte)pieceAtCurrentPosition % 4 == (byte)piece % 4)
                    break;

                var moveIsCapture = pieceAtCurrentPosition is not Piece.None;

                if (piece.HasFlag(Piece.Pawn))
                {
                    // Diagonal move without capture
                    if (Math.Abs(direction.X) == 1 &&
                        !(currentPosition == _enPassantAttackSquare) &&
                        !moveIsCapture)
                        break;

                    // Two-square moves from non-starting position
                    if (direction.Y == 2 && (square.Y != 1 || this[square+Up] != Piece.None || moveIsCapture))
                        break;

                    if (direction.Y == -2 && (square.Y != 6 || this[square+Down] != Piece.None || moveIsCapture))
                        break;

                    // Capturing forward
                    if (direction.Length() == 1 && moveIsCapture)
                        break;

                    // Promotion
                    if (currentPosition.Y % 7 == 0)
                    {
                        var pieceColor = piece.HasFlag(Piece.White) ? Piece.White : Piece.Black;
                        foreach (var pieceType in PromotionPieceTypes)
                        {
                            var promoteTo = pieceType | pieceColor;
                            moves.Add(new Move(
                                from: square,
                                to: currentPosition,
                                promotionTo: promoteTo));
                        }

                        break;
                    }
                }

                moves.Add(new Move(
                    from: square,
                    to: currentPosition));

                // Stop if the piece doesn't slide or we performed a capture
                if ((piece & (Piece.Bishop | Piece.Queen | Piece.Rook)) == 0 ||
                    moveIsCapture)
                    break;

                // Add castling moves when a rook slides next to the king
                if (!piece.HasFlag(Piece.Rook) || currentPosition.Y % 7 != 0)
                    continue;

                if (currentPosition == Squares.D1 &&
                    piece.HasFlag(Piece.White) &&
                    _castlingPrivileges.WhiteQueenSide)
                    moves.Add(new Move(Squares.E1, Squares.C1));

                if (currentPosition == Squares.F1 &&
                    piece.HasFlag(Piece.White) &&
                    _castlingPrivileges.WhiteKingSide)
                    moves.Add(new Move(Squares.E1, Squares.G1));

                if (currentPosition == Squares.D8 &&
                    piece.HasFlag(Piece.Black) &&
                    _castlingPrivileges.BlackQueenSide)
                    moves.Add(new Move(Squares.E8, Squares.C8));

                if (currentPosition == Squares.F8 &&
                    piece.HasFlag(Piece.Black) &&
                    _castlingPrivileges.BlackKingSide)
                    moves.Add(new Move(Squares.E8, Squares.G8));
            }
        }
    }

    private Vector[] GetMoveVectorsForPiece(Piece piece)
    {
        return piece switch
        {
            Piece.WhiteKing or Piece.BlackKing   => KingMoveDirections,
            Piece.WhiteQueen or Piece.BlackQueen  => QueenMoveDirections,
            Piece.WhiteRook or Piece.BlackRook   => RookMoveDirections,
            Piece.WhiteBishop or Piece.BlackBishop => BishopMoveDirections,
            Piece.WhiteKnight or Piece.BlackKnight => KnightMoveDirections,
            Piece.WhitePawn or Piece.BlackPawn   => piece.HasFlag(Piece.White)
                ? WhitePawnMoveDirections
                : BlackPawnMoveDirections,
            _ => throw new ArgumentOutOfRangeException($"Unknown piece type {piece}"),
        };
    }
}
