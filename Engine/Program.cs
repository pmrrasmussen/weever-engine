// See https://aka.ms/new-console-template for more information

namespace Engine;

public class Program
{
    public static void Main()
    {
        var searcher = new DepthSearcher();
        var engine = new UciEngine(Console.In, Console.Out, searcher);

        engine.Run(CancellationToken.None);
    }
}
