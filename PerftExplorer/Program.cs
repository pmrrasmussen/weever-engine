// See https://aka.ms/new-console-template for more information

using Chess.Builders;
using Chess.Structs;

if (!int.TryParse(args[0], out var depth))
    Console.WriteLine("Must pass an integer for the depth argument.");

var fen = args[1];

// var moves = args[2..].Select(moveString => new Move(moveString));

var board = BoardBuilder.FromFen(fen);
