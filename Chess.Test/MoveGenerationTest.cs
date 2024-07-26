using Chess.Enums;
using Chess.Structs;
using Xunit.Abstractions;

namespace Chess.Test;

public class MoveGenerationTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public MoveGenerationTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void GetLegalMoves_WhenSinglePawn_ShouldFindLegalMoves()
    {
        var board = new Board();

        board[Squares.D1] = new Piece(PieceType.Queen, Color.White);
        board[Squares.B3] = new Piece(PieceType.Pawn, Color.White);

        var moves = board.GetLegalMoves();
        _testOutputHelper.WriteLine(board.ToString());

        board.MakeMove(moves.First());
        _testOutputHelper.WriteLine(board.ToString());

        board.UndoLastMove();

        _testOutputHelper.WriteLine(board.ToString());
    }
}
