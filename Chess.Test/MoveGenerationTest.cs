using Chess.Enums;
using Chess.Structs;

namespace Chess.Test;

public class MoveGenerationTest
{
    [Fact]
    public void GetLegalMoves_WhenSinglePawn_ShouldFindLegalMoves()
    {
        var board = new Board();

        board[Squares.D1] = new Piece(PieceType.Queen, Color.White);
        board[Squares.B3] = new Piece(PieceType.Pawn, Color.White);

        var moves = board.GetLegalMoves();
    }
}
