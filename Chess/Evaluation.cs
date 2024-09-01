#define Evaluation

using Chess.Enums;

namespace Chess;

public partial class Board
{
    private int _middlegameEvaluation;
    private int _endgameEvaluation;
    private int _gamePhase;

    public int GetEvaluation()
    {
        var middleGamePhase = Math.Min(PestoEvaluationTables.MaximumGamePhase, _gamePhase);
        var endGamePhase = PestoEvaluationTables.MaximumGamePhase - middleGamePhase;
        var colorMultiplier = WhiteToMove ? 1 : -1;

        return colorMultiplier * (middleGamePhase * _middlegameEvaluation + endGamePhase * _endgameEvaluation)/PestoEvaluationTables.MaximumGamePhase;
    }

    public void RecomputeEvaluation()
    {
        _middlegameEvaluation = 0;
        _endgameEvaluation = 0;
        _gamePhase = 0;

        foreach (var square in BoardSquares.All)
        {
            var piece = this[square];
            if (piece == Piece.Empty)
                continue;

            _middlegameEvaluation -= PestoEvaluationTables.MiddlegameTables[(int)piece][(int)square];
            _endgameEvaluation -= PestoEvaluationTables.EndgameTables[(int)piece][(int)square];
            _gamePhase += PestoEvaluationTables.GamePhaseTable[(int)(piece & Piece.TypeMask)];
        }
    }

    private void EvaluateReplacingPieceOnSquare(Square square, Piece newPiece)
    {
#if Evaluation
        var originalPiece = this[square];
        if (originalPiece != Piece.Empty)
        {
            _middlegameEvaluation -= PestoEvaluationTables.MiddlegameTables[(int)originalPiece][(int)square];
            _endgameEvaluation -= PestoEvaluationTables.EndgameTables[(int)originalPiece][(int)square];
        }

        if (newPiece != Piece.Empty)
        {
            _middlegameEvaluation += PestoEvaluationTables.MiddlegameTables[(int)newPiece][(int)square];
            _endgameEvaluation += PestoEvaluationTables.EndgameTables[(int)newPiece][(int)square];
        }
#endif
    }

    private void UpdateGamePhase(Square moveTo, Piece movedPiece, Piece placedPiece)
    {
#if Evaluation
        if (this[moveTo] != Piece.Empty)
        {
            _gamePhase -= PestoEvaluationTables.GamePhaseTable[(int)(this[moveTo] & Piece.TypeMask)];
        }

        if (movedPiece != placedPiece)
        {
            _gamePhase += PestoEvaluationTables.GamePhaseTable[(int)(placedPiece & Piece.TypeMask)] - PestoEvaluationTables.GamePhaseTable[(int)(movedPiece & Piece.TypeMask)];
        }
#endif
    }
}
