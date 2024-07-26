using Chess.Enums;
using Chess.Structs;

namespace Chess.Builders;

public class BoardBuilder
{
    private const string StartingPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    public static Board GetDefaultStartingPosition()
    {
        return FromFen(StartingPositionFen);
    }

    public static Board FromFen(string fen)
    {
        var fenComponents = fen.Split();
        var (pieces, colorToMove, castlingPrivileges, enPassantSquare) =
        (
            fenComponents[0],
            fenComponents[1],
            fenComponents[2],
            fenComponents[3]
        );

        var board = new Board();

        var y = 7;
        foreach (var row in pieces.Split('/'))
        {
            var x = 0;
            foreach (var entry in row)
            {
                if (int.TryParse(entry.ToString(), out var skip))
                {
                    x += skip;
                    continue;
                }

                var square = new Square(x, y);
                var piece = GetPieceFromFen(entry);

                board[square] = piece;
                x++;
            }

            y--;
        }

        board.ColorToMove = GetColorToMoveFromFen(colorToMove);
        board.CastlingPrivileges = GetCastlingPrivilegesFromFen(castlingPrivileges);
        board.EnPassantAttackSquare = GetEnPassantAttackSquareFromFen(enPassantSquare);

        board.UpdateKingPositions();

        return board;
    }

    private static Piece GetPieceFromFen(char piece)
    {
        return piece switch
        {
            'K' => new Piece(PieceType.King, Color.White),
            'Q' => new Piece(PieceType.Queen, Color.White),
            'R' => new Piece(PieceType.Rook, Color.White),
            'B' => new Piece(PieceType.Bishop, Color.White),
            'N' => new Piece(PieceType.Knight, Color.White),
            'P' => new Piece(PieceType.Pawn, Color.White),
            'k' => new Piece(PieceType.King, Color.Black),
            'q' => new Piece(PieceType.Queen, Color.Black),
            'r' => new Piece(PieceType.Rook, Color.Black),
            'b' => new Piece(PieceType.Bishop, Color.Black),
            'n' => new Piece(PieceType.Knight, Color.Black),
            'p' => new Piece(PieceType.Pawn, Color.Black),
        };
    }

    private static Color GetColorToMoveFromFen(string fenColorToMove)
    {
        return fenColorToMove.Equals("w", StringComparison.OrdinalIgnoreCase)
            ? Color.White
            : Color.Black;
    }

    private static CastlingPrivileges GetCastlingPrivilegesFromFen(string fenCastlingPrivileges)
    {
        return new CastlingPrivileges(
            whiteKingSide: fenCastlingPrivileges.Contains('K'),
            whiteQueenSide: fenCastlingPrivileges.Contains('Q'),
            blackKingSide: fenCastlingPrivileges.Contains('k'),
            blackQueenSide: fenCastlingPrivileges.Contains('q'));
    }

    private static Square GetEnPassantAttackSquareFromFen(string fenEnPassantSquare)
    {
        if (fenEnPassantSquare.Equals("-", StringComparison.OrdinalIgnoreCase))
            return new Square();

        return Square.FromString(fenEnPassantSquare);
    }
}
