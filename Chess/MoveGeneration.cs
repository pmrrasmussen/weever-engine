using Chess.Enums;
using Chess.Structs;

namespace Chess;

public partial class Board
{
    private static readonly Vector Up = new (0, 1);
    private static readonly Vector Down = new (0, -1);
    private static readonly Vector Left = new (-1, 0);
    private static readonly Vector Right = new (1, 0);

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
        var forwardsDirection = color is Piece.White ? Up : Down;
        var kingPosition = _kingPositions[color.KingPositionIndex()];

        if (IsOppositeColouredPawn(color, kingPosition + forwardsDirection + Right) ||
            IsOppositeColouredPawn(color, kingPosition + forwardsDirection + Left))
            return true;

        foreach (var moveDirection in AllMoveDirections)
        {
            var directionLength = moveDirection.Length();

            var currentPosition = kingPosition;
            var stepCount = 0;
            while ((currentPosition += moveDirection).IsWithinBoard())
            {
                stepCount++;
                var piece = this[currentPosition];
                if (piece == Piece.None)
                {
                    if (directionLength == 3)
                        break;

                    continue;
                }

                if (piece.HasFlag(color))
                    break;

                var pieceType = piece & Piece.TypeMask;

                if (directionLength == 3 && pieceType == Piece.Knight)
                    return true;

                switch (directionLength, pieceType)
                {
                    case (< 3, Piece.King) when stepCount == 1:
                    case (< 3, Piece.Queen):
                    case (1, Piece.Rook):
                    case (2, Piece.Bishop):
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
        return (piece | color) == (Piece.Pawn | Piece.ColorMask);
    }

    public List<Move> GetLegalMoves()
    {
        var pseudoLegalMoves = new List<Move>();

        foreach (var square in Squares.All)
        {
            var piece = this[square];
            if (!piece.HasFlag(_colorToMove))
                continue;

            AddPseudoLegalPieceMoves(piece, square, pseudoLegalMoves);
        }

        var moves = new List<Move>(pseudoLegalMoves.Count);
        var fromSquaresToWatch = FromSquaresToWatchForChecks();
        var isCheck = IsCheck(_colorToMove);

        foreach (var move in pseudoLegalMoves)
        {
            if ((isCheck ||
                fromSquaresToWatch.Contains(move.From) ||
                move.To == _enPassantAttackSquare) &&
                MovesIntoCheck(move))
                continue;
            moves.Add(move);
        }

        return moves;
    }

    private List<Square> FromSquaresToWatchForChecks()
    {
        var kingSquare = _kingPositions[_colorToMove.KingPositionIndex()];
        List<Square> watchSquares = [ kingSquare ];

        foreach (var direction in QueenMoveDirections)
        {
            var currentSquare = kingSquare;
            var piece = Piece.None;
            while ((currentSquare += direction).IsWithinBoard() && (piece = this[currentSquare]) == Piece.None)
            {
            }

            if (piece.HasFlag(_colorToMove))
            {
                watchSquares.Add(currentSquare);
            }
        }

        return watchSquares;
    }

    private bool MovesIntoCheck(Move move)
    {
        bool isCheck;

        var currentColor = _colorToMove;
        var movedPiece = this[move.From];

        // Handle castling through check
        if ((movedPiece & Piece.TypeMask) is Piece.King && Math.Abs((move.To - move.From).X) == 2)
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
        var pieceType = piece & Piece.TypeMask;
        var pieceColor = piece & Piece.ColorMask;

        var directions = GetMoveVectorsForPiece(piece);

        foreach (var direction in directions)
        {
            var currentPosition = square;

            while ((currentPosition += direction).IsWithinBoard())
            {
                var pieceAtCurrentPosition = this[currentPosition];

                // Stop if we hit our own piece
                if (pieceAtCurrentPosition != Piece.None &&
                    pieceAtCurrentPosition.HasFlag(pieceColor))
                    break;

                var moveIsCapture = pieceAtCurrentPosition != Piece.None;

                if (pieceType is Piece.Pawn)
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
                        foreach (var promotionPieceType in PromotionPieceTypes)
                        {
                            var promoteTo = promotionPieceType | pieceColor;
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
                if (pieceType is Piece.Pawn or Piece.Knight or Piece.King ||
                    moveIsCapture)
                    break;

                // Add castling moves when a rook slides next to the king
                if (pieceType is not Piece.Rook || currentPosition.Y % 7 != 0)
                    continue;

                if (currentPosition == Squares.D1 &&
                    square == Squares.A1 &&
                    pieceColor is Piece.White &&
                    _castlingPrivileges.HasFlag(CastlingPrivileges.WhiteQueenSide))
                    moves.Add(new Move(Squares.E1, Squares.C1));

                if (currentPosition == Squares.F1 &&
                    square == Squares.H1 &&
                    pieceColor is Piece.White &&
                    _castlingPrivileges.HasFlag(CastlingPrivileges.WhiteKingSide))
                    moves.Add(new Move(Squares.E1, Squares.G1));

                if (currentPosition == Squares.D8 &&
                    square == Squares.A8 &&
                    pieceColor is Piece.Black &&
                    _castlingPrivileges.HasFlag(CastlingPrivileges.BlackQueenSide))
                    moves.Add(new Move(Squares.E8, Squares.C8));

                if (currentPosition == Squares.F8 &&
                    square == Squares.H8 &&
                    pieceColor is Piece.Black &&
                    _castlingPrivileges.HasFlag(CastlingPrivileges.BlackKingSide))
                    moves.Add(new Move(Squares.E8, Squares.G8));
            }
        }
    }

    private Vector[] GetMoveVectorsForPiece(Piece piece)
    {
        return piece switch
        {
            Piece.WhiteKing or Piece.BlackKing     => KingMoveDirections,
            Piece.WhiteQueen or Piece.BlackQueen   => QueenMoveDirections,
            Piece.WhiteRook or Piece.BlackRook     => RookMoveDirections,
            Piece.WhiteBishop or Piece.BlackBishop => BishopMoveDirections,
            Piece.WhiteKnight or Piece.BlackKnight => KnightMoveDirections,
            Piece.WhitePawn                        => WhitePawnMoveDirections,
            Piece.BlackPawn                        => BlackPawnMoveDirections,
            _ => throw new ArgumentOutOfRangeException($"Unknown piece type {piece}"),
        };
    }
}
