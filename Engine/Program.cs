// See https://aka.ms/new-console-template for more information

namespace Engine;

public class Program
{
    public static async Task Main()
    {
        var searcher = new Searcher();
        var engine = new UciEngine(Console.In, Console.Out, searcher);

        await engine.Start();
    }
}
