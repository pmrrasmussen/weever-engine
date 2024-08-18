using Chess.Enums;
using Chess.Structs;

namespace Chess;

public partial class Board
{
    private const int MaximumGamePhase = 24;
    private const bool EvaluationActive = false;

    private int _middleGameEvaluation = 0;
    private int _endGameEvaluation = 0;
    private int _gamePhase = 0;

    public int GetEvaluation()
    {
        var middleGamePhase = Math.Min(MaximumGamePhase, _gamePhase);
        var endGamePhase = MaximumGamePhase - middleGamePhase;

        return (middleGamePhase * _middleGameEvaluation + endGamePhase * _endGameEvaluation)/MaximumGamePhase;
    }

    private (int middleGameValue, int endGameValue) GetSquareValue(Square square, Piece piece)
    {
        return (piece.MiddleGameValue(), piece.EndGameValue());
    }
}
