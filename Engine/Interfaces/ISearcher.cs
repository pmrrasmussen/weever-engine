using Chess.Structs;

namespace Engine.Interfaces;

public interface ISearcher
{
    void SetPosition(string fen, IEnumerable<Move> moves);

    (Move move, int evaluation) Search(int remainingTimeInMilliseconds, CancellationToken cancellationToken);
}
