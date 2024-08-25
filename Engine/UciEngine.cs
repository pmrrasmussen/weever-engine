using Chess.Exceptions;
using Chess.Structs;
using Engine.Exceptions;
using Engine.Interfaces;

namespace Engine;

public class UciEngine(
    TextReader input,
    TextWriter output,
    ISearcher searcher)
{
    private Task _currentSearch = Task.CompletedTask;
    private bool _stopEngine;
    private bool _killSearch;
    private bool _debugModeActive = false;

    private CancellationTokenSource _searchTokenSource = new ();

    public void Run(CancellationToken cancellationToken)
    {
        while (!_stopEngine && !cancellationToken.IsCancellationRequested)
        {
            var input1 = input.ReadLine() ?? "";
            input1 = input1.Trim();

            if (input1 == "")
                continue;

            var command = input1.Split(" ")[0];
            var arguments = input1.Split(" ")[1..];

            switch (command.ToLower())
            {
                case "uci":
                    Id();
                    UciOk();
                    break;
                case "debug":
                    RegisterDebugMode(arguments);
                    break;
                case "isready":
                    ReadyOk();
                    break;
                case "setoption":
                    break;
                case "register":
                    break;
                case "ucinewgame":
                    ResetEngine();
                    break;
                case "position":
                    try
                    {
                        SetUpPosition(arguments);
                    }
                    catch (InvalidPositionException e)
                    {
                        Console.WriteLine($"Invalid format: {e.Message}\nPlease use 'position [fen  | startpos ]  moves'.");
                    }
                    break;
                case "go":
                    Go(arguments);
                    break;
                case "stop":
                    EndSearch();
                    break;
                case "quit":
                    EndSearch();
                    Stop();
                    break;
                default:
                    Console.WriteLine($"Unknown command: {command}.");
                    break;
            }

        }
    }

    private void Stop()
    {
        _stopEngine = true;
    }

    private void InvalidFormat(string errorMessage, string correctFormat)
    {
        output.WriteLine($"Invalid format: {errorMessage}\nPlease use '{correctFormat}'");
    }

    private void UciOk()
    {
        output.WriteLine("uciok");
    }

    private void Id()
    {
        output.WriteLine("id name Weever");
    }

    private void ReadyOk()
    {
        output.WriteLine("readyok");
    }

    private void BestMove(Move bestMove)
    {
        output.WriteLine($"bestmove {bestMove.ToString().ToLower()}");
    }

    private void ResetEngine()
    {
        KilLSearch();
    }

    private void Go(string[] arguments)
    {
        if (_currentSearch.Status == TaskStatus.Running)
            output.WriteLine("Invalid command. Search is already running.");

        _searchTokenSource = new CancellationTokenSource();
        var cancellationToken = _searchTokenSource.Token;
        _searchTokenSource.CancelAfter(2000);

        _currentSearch = arguments.Length == 0
            ? Task.Run(() => InfiniteSearch(cancellationToken), cancellationToken)
            : Task.Run(() => Search(arguments, cancellationToken), cancellationToken);
    }

    private void EndSearch()
    {
        _searchTokenSource.Cancel();
    }

    private void KilLSearch()
    {
        if (_currentSearch.Status != TaskStatus.Running)
            return;

        _killSearch = true;
        EndSearch();
    }

    private void InfiniteSearch(CancellationToken cancellationToken)
    {
        var bestMove = searcher.Search(int.MaxValue, cancellationToken);

        if (!_killSearch)
            BestMove(bestMove);

        _killSearch = false;
    }

    private  void Search(string[] arguments, CancellationToken cancellationToken)
    {
        var bestMove = searcher.Search(3000, cancellationToken);

        if (!_killSearch)
            BestMove(bestMove);

        _killSearch = false;
    }

    private void RegisterDebugMode(string[] arguments)
    {
        if (arguments.Length != 1 || arguments[0] is not ("on" or "off"))
            InvalidFormat($"Bad arguments {arguments}", "debug [ on | off ]");
        else
            _debugModeActive = arguments[0] == "on";
    }

    private void SetUpPosition(string[] arguments)
    {
        if (arguments.Length == 0)
            throw new InvalidPositionException("No arguments specified.");

        var positionType = arguments[0];
        var remainingArguments = arguments[1..];
        var fen = "";
        switch (positionType)
        {
            case "startpos":
                break;
            case "fen":
                if (remainingArguments.Length == 0)
                    throw new InvalidPositionException("Fen string is missing.");

                fen = remainingArguments[0];
                remainingArguments = remainingArguments[1..];
                break;
            default:
                throw new InvalidPositionException($"Invalid position argument {positionType}.");
        }

        string[] moveStrings = [];
        if (remainingArguments.Length > 0)
        {
            if (!remainingArguments[0].Equals("moves", StringComparison.OrdinalIgnoreCase))
                throw new InvalidPositionException($"Invalid argument {remainingArguments[0]} was expecting 'moves'");

            moveStrings = remainingArguments[1..];
        }

        List<Move> moves;
        try
        {
            moves = moveStrings.Select(s => s.ToMove()).ToList();
        }
        catch (InvalidMoveException e)
        {
            throw new InvalidPositionException($"Invalid moves. Error message: {e.Message}.");
        }

        try
        {
            searcher.SetPosition(fen, moves);
        }
        catch (Exception e)
        {
            throw new InvalidPositionException($"Failed to load position from arguments {arguments}. Error message: {e.Message}");
        }

    }
}
