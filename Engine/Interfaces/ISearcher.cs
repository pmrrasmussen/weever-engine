using Chess.Structs;

namespace Engine.Interfaces;

public interface ISearcher
{
    void SetPosition(string fen, IEnumerable<Move> moves);

    Task<Move> InfiniteSearch();

    Task<Move> Search(int remainingTimeInMilliseconds);

    void EndSearch();
}
