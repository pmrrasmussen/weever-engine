using Chess.Enums;
using Chess.Structs;

namespace Chess.Builders;

public static class BoardBuilder
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

                var square = (Square)(x + 1) + (y + 2) * 10;
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
            'K' => Piece.WhiteKing,
            'Q' => Piece.WhiteQueen,
            'R' => Piece.WhiteRook,
            'B' => Piece.WhiteBishop,
            'N' => Piece.WhiteKnight,
            'P' => Piece.WhitePawn,
            'k' => Piece.BlackKing,
            'q' => Piece.BlackQueen,
            'r' => Piece.BlackRook,
            'b' => Piece.BlackBishop,
            'n' => Piece.BlackKnight,
            'p' => Piece.BlackPawn,
            _ => throw new ArgumentOutOfRangeException($"Unrecognised piece {piece}"),
        };
    }

    private static Piece GetColorToMoveFromFen(string fenColorToMove)
    {
        return fenColorToMove.Equals("w", StringComparison.OrdinalIgnoreCase)
            ? Piece.White
            : Piece.Black;
    }

    private static CastlingPrivileges GetCastlingPrivilegesFromFen(string fenCastlingPrivileges)
    {
        var castlingPrivileges = CastlingPrivileges.None;

        if (fenCastlingPrivileges.Contains('K'))
            castlingPrivileges |= CastlingPrivileges.WhiteKingSide;

        if (fenCastlingPrivileges.Contains('Q'))
            castlingPrivileges |= CastlingPrivileges.WhiteQueenSide;

        if (fenCastlingPrivileges.Contains('k'))
            castlingPrivileges |= CastlingPrivileges.BlackKingSide;

        if (fenCastlingPrivileges.Contains('q'))
            castlingPrivileges |= CastlingPrivileges.BlackQueenSide;

        return castlingPrivileges;
    }

    private static Square GetEnPassantAttackSquareFromFen(string fenEnPassantSquare)
    {
        return fenEnPassantSquare.Equals("-", StringComparison.OrdinalIgnoreCase)
            ? Square.NullSquare
            : fenEnPassantSquare.ToSquare();
    }
}
