using Chess.Builders;
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
        var board = BoardBuilder.GetDefaultStartingPosition();

        var moves = board.GetPseudoLegalMoves();
        _testOutputHelper.WriteLine(board.ToString());

        foreach (var move in moves)
        {
            board.MakeMove(move);
            _testOutputHelper.WriteLine(board.ToString());
            _testOutputHelper.WriteLine(move.ToString());
            board.UndoLastMove();
        }

        _testOutputHelper.WriteLine(board.ToString());
    }
}
