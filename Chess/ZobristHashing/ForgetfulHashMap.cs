namespace Chess.ZobristHashing;

public class ForgetfulHashMap<T>(long capacity)
{
   private readonly (T element, long hash)[] _table = new (T element, long hash)[capacity];

   public void Add(T element, long hash)
   {
      _table[hash % capacity] = (element, hash);
   }

   public bool TryGetValue(long hash, out T element)
   {
      (element, var tableHash) = _table[hash % capacity];

      return tableHash == hash;
   }
}
