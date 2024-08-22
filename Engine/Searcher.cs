using Chess.Structs;
using Engine.Interfaces;

namespace Engine;

public class Searcher : ISearcher
{
    public void SetPosition(string fen, IEnumerable<Move> moves)
    {
        throw new NotImplementedException();
    }

    public Task<Move> InfiniteSearch()
    {
        throw new NotImplementedException();
    }

    public Task<Move> Search(int remainingTimeInMilliseconds)
    {
        throw new NotImplementedException();
    }

    public void EndSearch()
    {
        throw new NotImplementedException();
    }
}
