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

    private static readonly int[][] MoveDirections =
    [
        BlackPawnMoveDirections,
        KnightMoveDirections,
        BishopMoveDirections,
        RookMoveDirections,
        QueenMoveDirections,
        KingMoveDirections,
        WhitePawnMoveDirections,
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

    public bool IsPlayerToMoveInCheck()
    {
        return IsCheck(_colorToMove);
    }

    private bool IsCheck(Piece color)
    {
        var enemyColor = color ^ Piece.ColorMask;
        var enemyForwardDirection = enemyColor is Piece.White ? Up : Down;
        var kingPosition = _kingPositions[color.KingPositionIndex()];

        if (this[kingPosition - (enemyForwardDirection + Right)] == (Piece.Pawn | enemyColor) ||
            this[kingPosition - (enemyForwardDirection + Left)] == (Piece.Pawn | enemyColor))
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

                    var pieceType = piece & Piece.TypeMask;
                    var pieceColor = piece & Piece.ColorMask;

                    if (pieceColor != enemyColor)
                        break;

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

    private bool IsPawnOfColor(Piece color, Square square)
    {
        return this[square] == (Piece.Pawn | color);
    }

    public List<Move> GetLegalMoves()
    {
        var pseudoLegalMoves = new List<Move>();

        foreach (var square in BoardSquares.All)
        {
            var piece = this[square];
            if (!piece.HasFlag(_colorToMove))
                continue;

            AddPseudoLegalPieceMoves(piece, square, pseudoLegalMoves);
        }

        var moves = new List<Move>(pseudoLegalMoves.Count);
        var squaresOfPotentiallyPinnedPieces = GetSquaresOfPotentiallyPinnedPieces();
        var isCheck = IsCheck(_colorToMove);

        foreach (var move in pseudoLegalMoves)
        {
            var mustCheckMoveLegality =
                isCheck ||
                squaresOfPotentiallyPinnedPieces.Contains(move.From) ||
                move.To == _enPassantAttackSquare;

            if (!mustCheckMoveLegality || !IsMoveIntoCheck(move))
                moves.Add(move);
        }

        return moves;
    }

    private List<Square> GetSquaresOfPotentiallyPinnedPieces()
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

    private bool IsMoveIntoCheck(Move move)
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

    private void AddPseudoLegalPieceMoves(Piece movedPiece, Square fromSquare, List<Move> moves)
    {
        var movedPieceType = movedPiece & Piece.TypeMask;
        var movedPieceColor = movedPiece & Piece.ColorMask;

        var index = movedPiece == Piece.WhitePawn ? 6 : (int)movedPieceType;
        var directions = MoveDirections[index];

        foreach (var direction in directions)
        {
            var currentSquare = fromSquare;
            Piece pieceAtCurrentSquare;
            while ((pieceAtCurrentSquare = this[currentSquare += direction]) != Piece.OutOfBounds)
            {
                var moveIsCapture = pieceAtCurrentSquare != Piece.Empty;

                // Stop if we hit our own piece
                if (moveIsCapture && pieceAtCurrentSquare.HasFlag(movedPieceColor))
                    break;

                if (movedPieceType is Piece.Pawn)
                {
                    // Forward moves
                    if (direction % 10 == 0)
                    {
                        if (moveIsCapture)
                            break;

                        if (Math.Abs(direction) == 20 &&
                            (fromSquare.GetRank() is not (WhiteBackRank + 1 or BlackBackRank - 1) ||
                             this[fromSquare + direction/2] != Piece.Empty))
                            break;
                    }
                    // Diagonal moves
                    else
                    {
                        if (currentSquare != _enPassantAttackSquare && !moveIsCapture)
                            break;
                    }

                    // Promotion
                    if (currentSquare.GetRank() is WhiteBackRank or BlackBackRank)
                    {
                        foreach (var promotionPieceType in PromotionPieceTypes)
                        {
                            var promoteTo = promotionPieceType | movedPieceColor;
                            moves.Add(new Move(
                                from: fromSquare,
                                to: currentSquare,
                                promotionTo: promoteTo));
                        }

                        break;
                    }
                }

                moves.Add(new Move(
                    from: fromSquare,
                    to: currentSquare));

                // Stop if the piece doesn't slide or we performed a capture
                if (movedPieceType is Piece.Pawn or Piece.Knight or Piece.King ||
                    moveIsCapture)
                    break;

                // Add castling moves when a rook slides next to the king
                if (movedPieceType is not Piece.Rook)
                    continue;

                switch (currentPosition: currentSquare, square: fromSquare, pieceColor: movedPieceColor)
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
}
