using System.Diagnostics;
using Chess.Enums;

namespace Chess.Structs;

public readonly struct Piece(PieceType type, Color color)
{
    public readonly PieceType Type = type;

    public readonly Color Color = color;

    public override string ToString()
    {
        return (Type, Color) switch
        {
            (PieceType.King, Color.White) => "\u2654",
            (PieceType.King, Color.Black) => "\u265a",
            (PieceType.Queen, Color.White) => "\u2655",
            (PieceType.Queen, Color.Black) => "\u265b",
            (PieceType.Rook, Color.White) => "\u2656",
            (PieceType.Rook, Color.Black) => "\u265c",
            (PieceType.Bishop, Color.White) => "\u2657",
            (PieceType.Bishop, Color.Black) => "\u265d",
            (PieceType.Knight, Color.White) => "\u2658",
            (PieceType.Knight, Color.Black) => "\u265e",
            (PieceType.Pawn, Color.White) => "\u2659",
            (PieceType.Pawn, Color.Black) => "\u265f",
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
