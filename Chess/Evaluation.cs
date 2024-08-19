#define Evaluation

using Chess.Enums;

namespace Chess;

public partial class Board
{
    private const int MaximumGamePhase = 24;
    private int _middleGameEvaluation;
    private int _endGameEvaluation;
    private int _gamePhase;

    private static int[][] _middleGameTables = Enumerable.Repeat(Enumerable.Repeat(5, 120).ToArray(), 22).ToArray();
    private static int[][] _endGameTables = Enumerable.Repeat(Enumerable.Repeat(10, 120).ToArray(), 22).ToArray();
    private static int[] _gamePhaseTable = Enumerable.Repeat(0, 20).ToArray();

    public int GetEvaluation()
    {
        var middleGamePhase = Math.Min(MaximumGamePhase, _gamePhase);
        var endGamePhase = MaximumGamePhase - middleGamePhase;

        return (middleGamePhase * _middleGameEvaluation + endGamePhase * _endGameEvaluation)/MaximumGamePhase;
    }

    private void EvaluateReplacingPieceOnSquare(Square square, Piece newPiece)
    {
#if Evaluation
        var originalPiece = this[square];
        if (originalPiece != Piece.Empty)
        {
            _middleGameEvaluation -= _middleGameTables[(int)originalPiece][(int)square];
            _endGameEvaluation -= _endGameTables[(int)originalPiece][(int)square];
        }

        _middleGameEvaluation += _middleGameTables[(int)newPiece][(int)square];
        _endGameEvaluation += _endGameTables[(int)newPiece][(int)square];
#endif
    }

    private void UpdateGamePhase(Square moveTo, Piece movedPiece, Piece placedPiece)
    {
#if Evaluation
        if (this[moveTo] != Piece.Empty)
        {
            _gamePhase -= _gamePhaseTable[(int)(this[moveTo] & Piece.TypeMask)];
        }

        if (movedPiece != placedPiece)
        {
            _gamePhase += _gamePhaseTable[(int)(placedPiece & Piece.TypeMask)] - _gamePhaseTable[(int)(movedPiece & Piece.TypeMask)];
        }
#endif
    }
}
