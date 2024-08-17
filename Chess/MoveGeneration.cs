using System.Runtime.CompilerServices;
using Chess.Enums;
using Chess.Structs;

namespace Chess;

public partial class Board
{
    private enum MoveDirectionType
    {
        Straight = 0,
        Diagonal = 1,
        Knight = 2,
    }

    private static readonly MoveDirectionType[] MoveDirectionTypes =
        [MoveDirectionType.Straight, MoveDirectionType.Diagonal, MoveDirectionType.Knight];

    private const int WhiteBackRank = 2;
    private const int BlackBackRank = 9;

    private const int Up = 10;
    private const int Down = -Up;
    private const int Left = -1;
    private const int Right = -Left;

    private static readonly int[] WhitePawnMoveDirections = [Up, Up + Left, Up + Right, 2 * Up];
    private static readonly int[] BlackPawnMoveDirections = WhitePawnMoveDirections.Select(d => -1 * d).ToArray();
    private static readonly int[] BishopMoveDirections = [Up + Left, Up + Right, Down + Left, Down + Right];
    private static readonly int[] KnightMoveDirections =
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
    private static readonly int[] RookMoveDirections = [Up, Left, Right, Down];
    private static readonly int[] QueenMoveDirections =
    [
        Up, Left, Right, Down, Up + Left, Up + Right, Down + Left, Down + Right
    ];
    private static readonly int[] KingMoveDirections =
    [
        Up, Left, Right, Down, Up + Left, Up + Right, Down + Left, Down + Right
    ];

    private static readonly int[][] AllMoveDirections =
    [
        RookMoveDirections,
        BishopMoveDirections,
        KnightMoveDirections,
    ];
    
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

        foreach (var moveDirectionType in MoveDirectionTypes)
        {
            foreach (var moveDirection in AllMoveDirections[(int)moveDirectionType])
            {
                var currentPosition = kingPosition;
                var stepCount = 0;
                Piece piece;
                while ((piece = this[currentPosition += moveDirection]) != Piece.OutOfBounds)
                {
                    stepCount++;
                    if (piece == Piece.Empty)
                    {
                        if (moveDirectionType == MoveDirectionType.Knight)
                            break;

                        continue;
                    }

                    if (piece.HasFlag(color))
                        break;

                    var pieceType = piece & Piece.TypeMask;

                    if (moveDirectionType == MoveDirectionType.Knight && pieceType == Piece.Knight)
                        return true;

                    switch (moveDirectionType, pieceType)
                    {
                        case (MoveDirectionType.Diagonal or MoveDirectionType.Straight, Piece.King) when stepCount == 1:
                        case (MoveDirectionType.Diagonal or MoveDirectionType.Straight, Piece.Queen):
                        case (MoveDirectionType.Straight, Piece.Rook):
                        case (MoveDirectionType.Diagonal, Piece.Bishop):
                            return true;
                    }

                    break;
                }

            }
        }

        return false;
    }

    private bool IsOppositeColouredPawn(Piece color, Square square)
    {
        return (this[square] | color) == (Piece.Pawn | Piece.ColorMask);
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
            Piece piece;
            while ((piece = this[currentSquare += direction]) == Piece.Empty)
            {
            }

            if (piece.HasFlag(_colorToMove))
                watchSquares.Add(currentSquare);
        }

        return watchSquares;
    }

    private bool MovesIntoCheck(Move move)
    {
        bool isCheck;

        var currentColor = _colorToMove;
        var movedPiece = this[move.From];

        // Handle castling through check
        if ((movedPiece & Piece.TypeMask) is Piece.King && Math.Abs(move.To - move.From) == 2)
        {
            if (IsCheck(currentColor))
                return true;

            var inBetweenSquare = (Square)(((int)move.To + (int)move.From) / 2);
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
            Piece pieceAtCurrentPosition;
            while ((pieceAtCurrentPosition = this[currentPosition += direction]) != Piece.OutOfBounds)
            {
                // Stop if we hit our own piece
                if (pieceAtCurrentPosition != Piece.Empty &&
                    pieceAtCurrentPosition.HasFlag(pieceColor))
                    break;

                var moveIsCapture = pieceAtCurrentPosition != Piece.Empty;

                if (pieceType is Piece.Pawn)
                {
                    // Diagonal move without capture
                    if (direction % 10 != 0 &&
                        currentPosition != _enPassantAttackSquare &&
                        !moveIsCapture)
                        break;

                    // Two-square moves from non-starting position
                    if (direction == 20 && (square.Rank() != WhiteBackRank + 1 || this[square+Up] != Piece.Empty || moveIsCapture))
                        break;

                    if (direction == -20 && (square.Rank() != BlackBackRank - 1 || this[square+Down] != Piece.Empty || moveIsCapture))
                        break;

                    // Capturing forward
                    if (Math.Abs(direction) == 10 && moveIsCapture)
                        break;

                    // Promotion
                    if (currentPosition.Rank() is WhiteBackRank or BlackBackRank)
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
                if (pieceType is not Piece.Rook || currentPosition.Rank() is not (WhiteBackRank or BlackBackRank))
                    continue;

                switch (currentPosition, square, pieceColor)
                {
                    case (Square.D1, Square.A1, Piece.White) when _castlingPrivileges.HasFlag(CastlingPrivileges.WhiteQueenSide):
                        moves.Add(new Move(Square.E1, Square.C1));
                        break;
                    case (Square.F1, Square.H1, Piece.White) when _castlingPrivileges.HasFlag(CastlingPrivileges.WhiteKingSide):
                        moves.Add(new Move(Square.E1, Square.G1));
                        break;
                    case (Square.D8, Square.A8, Piece.Black) when _castlingPrivileges.HasFlag(CastlingPrivileges.BlackQueenSide):
                        moves.Add(new Move(Square.E8, Square.C8));
                        break;
                    case (Square.F8, Square.H8, Piece.Black) when _castlingPrivileges.HasFlag(CastlingPrivileges.BlackKingSide):
                        moves.Add(new Move(Square.E8, Square.G8));
                        break;
                }
            }
        }
    }

    private int[] GetMoveVectorsForPiece(Piece piece)
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
