using Chess.Structs;
using Engine.Exceptions;
using Engine.Interfaces;

namespace Engine;

public class UciEngine
{
    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly ISearcher _searcher;

    private Task _currentSearch = Task.CompletedTask;
    private bool _stopEngine;
    private bool _killSearch;
    private bool _debugModeActive = false;

    public UciEngine(
        TextReader input,
        TextWriter output,
        ISearcher searcher)
    {
        _input = input;
        _output = output;
        _searcher = searcher;
    }

    public void Stop()
    {
        _stopEngine = true;
    }

    public async Task Start()
    {
        while (!_stopEngine)
        {
            string input = await _input.ReadLineAsync() ?? "";
            input = input.Trim();

            if (input == "")
                continue;

            var command = input.Split(" ")[0];
            var arguments = input.Split(" ")[1..];

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

    private void InvalidFormat(string errorMessage, string correctFormat)
    {
        _output.WriteLine($"Invalid format: {errorMessage}\nPlease use '{correctFormat}'");
    }

    private void UciOk()
    {
        _output.WriteLine("uciok");
    }

    private void Id()
    {
        _output.WriteLine("id name Weever");
    }

    private void ReadyOk()
    {
        _output.WriteLine("readyok");
    }

    private void BestMove(Move bestMove)
    {
        _output.WriteLine($"bestmove {bestMove.ToString()}");
    }

    private void ResetEngine()
    {
    }

    private void Go(string[] arguments)
    {
        if (_currentSearch.Status == TaskStatus.Running)
            _output.WriteLine("Invalid command. Search is already running.");
        if (arguments.Length == 0)
        {
            _currentSearch = NewInfiniteSearch();
            return;
        }

        _currentSearch = NewSearch(arguments);
    }

    private void EndSearch()
    {
        _searcher.EndSearch();
    }

    private void KilLSearch()
    {
        _killSearch = true;
        _searcher.EndSearch();
    }

    private async Task NewInfiniteSearch()
    {
        var bestMove = await _searcher.InfiniteSearch();

        if (!_killSearch)
            BestMove(bestMove);

        _killSearch = false;
    }

    private async Task NewSearch(string[] arguments)
    {
        var bestMove = await _searcher.Search(1000);

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

    private void SetUpPosition(string[] positionArguments)
    {
        if (positionArguments.Length == 0)
            throw new InvalidPositionException("No arguments specified.");

        var positionType = positionArguments[0];
        string fen = "";
        string[] moveStrings;
        switch (positionType)
        {
            case "startpos":
                moveStrings = positionArguments[1..];
                break;
            case "fen":
                if (positionArguments.Length == 1)
                    throw new InvalidPositionException("Fen string is missing.");

                fen = positionArguments[1];
                moveStrings = positionArguments[2..];
                break;
            default:
                throw new InvalidPositionException($"Invalid position argument {positionType}.");
        }

        var moves = new List<Move>();
        foreach (var moveString in moveStrings)
        {
            try
            {
                moves.Add(moveString.ToMove());
            }
            catch (Exception e)
            {
                throw new InvalidPositionException($"Invalid move {moveString}. Error message: {e.Message}.");
            }
        }

        try
        {
            _searcher.SetPosition(fen, moves);
        }
        catch (Exception e)
        {
            throw new InvalidPositionException($"Failed to load position from arguments {positionArguments[1]}. Error message: {e.Message}");
        }

    }
}
