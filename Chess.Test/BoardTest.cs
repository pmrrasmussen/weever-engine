using Chess.Builders;
using Chess.Structs;

namespace Chess.Test;

public class BoardTest
{

    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "e2e3 e7e5 d2d3", "e2e3 e7e5 d2d3")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "", "")]
    public void Equals_WhenBoardsAreEqual_ReturnsTrue(
        string fen,
        string moveOrder1,
        string moveOrder2)
    {
        // Arrange
        var board1 = BoardBuilder.FromFen(fen);
        var board2 = BoardBuilder.FromFen(fen);

        var moves1 = GetMoves(moveOrder1);
        var moves2 = GetMoves(moveOrder2);

        foreach (var move in moves1)
        {
            board1.MakeMove(move);
        }

        foreach (var move in moves2)
        {
            board2.MakeMove(move);
        }

        // Act
        var result = board1.Equals(board2);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "e2e3 e7e5 d2d3", "d2d3 e7e5 e2e3")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "g1f3 g8f6 f3g1 f6g8", "")]
    public void Equals_WhenBoardsAreEqualExceptForDifferentMoveHistory_ReturnsFalse(
        string fen,
        string moveOrder1,
        string moveOrder2)
    {
        // Arrange
        var board1 = BoardBuilder.FromFen(fen);
        var board2 = BoardBuilder.FromFen(fen);

        var moves1 = GetMoves(moveOrder1);
        var moves2 = GetMoves(moveOrder2);

        foreach (var move in moves1)
        {
            board1.MakeMove(move);
        }

        foreach (var move in moves2)
        {
            board2.MakeMove(move);
        }

        // Act
        var result = board1.Equals(board2);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "e2e3 e7e5 d2d3", "d2d3 e7e5 e2e3")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "g1f3 e7e5 f3g1", "g1h3 e7e5 h3g1")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "g1f3 g8f6 f3g1 f6g8", "")]
    public void HistoryAgnosticEquals_WhenBoardsAreSameButWithDifferentMoves_ReturnsTrue(
        string fen,
        string moveOrder1,
        string moveOrder2)
    {
        // Arrange
        var board1 = BoardBuilder.FromFen(fen);
        var board2 = BoardBuilder.FromFen(fen);

        var moves1 = GetMoves(moveOrder1);
        var moves2 = GetMoves(moveOrder2);

        foreach (var move in moves1)
        {
            board1.MakeMove(move);
        }

        foreach (var move in moves2)
        {
            board2.MakeMove(move);
        }

        // Act
        var result = board1.HistoryAgnosticEquals(board2);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "e2e4 e7e5 d2d4", "d2d4 e7e5 e2e4")]
    public void HistoryAgnosticEquals_WhenBoardsHaveDifferentEnPassantSquares_ReturnsFalse(
        string fen,
        string moveOrder1,
        string moveOrder2)
    {
        // Arrange
        var board1 = BoardBuilder.FromFen(fen);
        var board2 = BoardBuilder.FromFen(fen);

        var moves1 = GetMoves(moveOrder1);
        var moves2 = GetMoves(moveOrder2);

        foreach (var move in moves1)
        {
            board1.MakeMove(move);
        }

        foreach (var move in moves2)
        {
            board2.MakeMove(move);
        }

        // Act
        var result = board1.HistoryAgnosticEquals(board2);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("rnbqkbnr/ppppppp1/8/8/8/8/PPPPPPP1/RNBQKBNR w KQkq - 0 1", "h1h2 h8h7 h2h1 h7h8", "h1h2 h8h7 h2h3 h7h8 h3h1")]
    public void HistoryAgnosticEquals_WhenBoardsHaveDifferentPlayerToMove_ReturnsFalse(
        string fen,
        string moveOrder1,
        string moveOrder2)
    {
        // Arrange
        var board1 = BoardBuilder.FromFen(fen);
        var board2 = BoardBuilder.FromFen(fen);

        var moves1 = GetMoves(moveOrder1);
        var moves2 = GetMoves(moveOrder2);

        foreach (var move in moves1)
        {
            board1.MakeMove(move);
        }

        foreach (var move in moves2)
        {
            board2.MakeMove(move);
        }

        // Act
        var result = board1.HistoryAgnosticEquals(board2);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("rnbqkbnr/1ppp1pp1/8/8/8/8/1PPP1PP1/RNBQKBNR w KQkq - 0 1", "h1h2 g8f6 h2h1 f6g8", "")]
    [InlineData("rnbqkbnr/1ppp1pp1/8/8/8/8/1PPP1PP1/RNBQKBNR w KQkq - 0 1", "a1a2 g8f6 a2a1 f6g8", "")]
    [InlineData("rnbqkbnr/1ppp1pp1/8/8/8/8/1PPP1PP1/RNBQKBNR w KQkq - 0 1", "e1e2 g8f6 e2e1 f6g8", "")]
    [InlineData("rnbqkbnr/1ppp1pp1/8/8/8/8/1PPP1PP1/RNBQKBNR w KQkq - 0 1", "h8h7 g8f6 h7h8 f6g8", "")]
    [InlineData("rnbqkbnr/1ppp1pp1/8/8/8/8/1PPP1PP1/RNBQKBNR w KQkq - 0 1", "a8a7 g8f6 a7a8 f6g8", "")]
    [InlineData("rnbqkbnr/1ppp1pp1/8/8/8/8/1PPP1PP1/RNBQKBNR w KQkq - 0 1", "e8e7 g8f6 e7e8 f6g8", "")]
    public void HistoryAgnosticEquals_WhenBoardsHaveDifferentCastlingPrivileges_ReturnsFalse(
        string fen,
        string moveOrder1,
        string moveOrder2)
    {
        // Arrange
        var board1 = BoardBuilder.FromFen(fen);
        var board2 = BoardBuilder.FromFen(fen);

        var moves1 = GetMoves(moveOrder1);
        var moves2 = GetMoves(moveOrder2);

        foreach (var move in moves1)
        {
            board1.MakeMove(move);
        }

        foreach (var move in moves2)
        {
            board2.MakeMove(move);
        }

        // Act
        var result = board1.HistoryAgnosticEquals(board2);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "e2e3 e7e5 d2d3")]
    public void Clone_ReturnsEqualBoards(string fen, string moveString)
    {
        // Arrange
        var board = BoardBuilder.FromFen(fen);

        var moves = GetMoves(moveString);

        foreach (var move in moves)
        {
            board.MakeMove(move);
        }

        // Act
        var result = board.Clone();

        // Assert
        Assert.True(result.Equals(board));
    }

    private IEnumerable<Move> GetMoves(string moves)
    {
        if (moves.Length == 0)
            return Enumerable.Empty<Move>();

        return moves.Split().Select(m => m.ToMove());
    }
}
