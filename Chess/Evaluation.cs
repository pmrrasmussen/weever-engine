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
        var middleGamePhase = Math.Min(PeSTO.MaximumGamePhase, _gamePhase);
        var endGamePhase = PeSTO.MaximumGamePhase - middleGamePhase;

        return (middleGamePhase * _middlegameEvaluation + endGamePhase * _endgameEvaluation)/PeSTO.MaximumGamePhase;
    }

    public void RecomputeEvaluation()
    {
        _middlegameEvaluation = 0;
        _endgameEvaluation = 0;
        _gamePhase = 0;

        foreach (var square in Squares.All)
        {
            var piece = this[square];
            if (piece == Piece.Empty)
                continue;

            _middlegameEvaluation -= PeSTO.MiddlegameTables[(int)piece][(int)square];
            _endgameEvaluation -= PeSTO.EndgameTables[(int)piece][(int)square];
            _gamePhase += PeSTO.GamePhaseTable[(int)(piece & Piece.TypeMask)];
        }
    }

    private void EvaluateReplacingPieceOnSquare(Square square, Piece newPiece)
    {
#if Evaluation
        var originalPiece = this[square];
        if (originalPiece != Piece.Empty)
        {
            _middlegameEvaluation -= PeSTO.MiddlegameTables[(int)originalPiece][(int)square];
            _endgameEvaluation -= PeSTO.EndgameTables[(int)originalPiece][(int)square];
        }

        if (newPiece != Piece.Empty)
        {
            _middlegameEvaluation += PeSTO.MiddlegameTables[(int)newPiece][(int)square];
            _endgameEvaluation += PeSTO.EndgameTables[(int)newPiece][(int)square];
        }
#endif
    }

    private void UpdateGamePhase(Square moveTo, Piece movedPiece, Piece placedPiece)
    {
#if Evaluation
        if (this[moveTo] != Piece.Empty)
        {
            _gamePhase -= PeSTO.GamePhaseTable[(int)(this[moveTo] & Piece.TypeMask)];
        }

        if (movedPiece != placedPiece)
        {
            _gamePhase += PeSTO.GamePhaseTable[(int)(placedPiece & Piece.TypeMask)] - PeSTO.GamePhaseTable[(int)(movedPiece & Piece.TypeMask)];
        }
#endif
    }
}
