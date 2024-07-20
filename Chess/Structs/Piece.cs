using Chess.Enums;

namespace Chess.Structs;

public readonly struct Piece(PieceType type, Color color)
{
    public readonly PieceType Type = type;

    public readonly Color Color = color;
}
