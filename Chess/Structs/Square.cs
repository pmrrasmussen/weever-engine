namespace Chess.Structs;

public readonly struct Square(int x, int y) : IEquatable<Square>
{
    public readonly int X = x;

    public readonly int Y = y;

    public static Square operator +(Square square, Vector vector)
        => new (square.X + vector.X, square.Y + vector.Y);

    public static Vector operator -(Square a, Square b)
        => new (a.X - b.X, a.Y - b.Y);
    public override bool Equals(object? obj)
        => obj is Square other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(X, Y);

    public bool Equals(Square otherSquare)
        => X == otherSquare.X && Y == otherSquare.Y;

    public static bool operator ==(Square a, Square b)
        => a.Equals(b);

    public static bool operator !=(Square a, Square b)
        => !a.Equals(b);
}

public readonly struct Vector(int x, int y)
{
    public readonly int X = x;

    public readonly int Y = y;

    public static Vector operator +(Vector vector1, Vector vector2)
        => new (vector1.X + vector2.X, vector1.Y + vector2.Y);

    public static Vector operator *(int scalar, Vector vector)
        => new (scalar * vector.X, scalar * vector.Y);

    public int Length() => Math.Abs(X) + Math.Abs(Y);
}

public static class SquareExtensions
{
    public static bool IsWithinBoard(this Square square)
    {
        return square is { X: >= 0 and < 8, Y: >= 0 and < 8 };
    }
}

public static class Squares
{
    public static readonly Square NullSquare = new (-2, -2);

    public static readonly Square A1 = new Square(0, 0);
    public static readonly Square A2 = new Square(0, 1);
    public static readonly Square A3 = new Square(0, 2);
    public static readonly Square A4 = new Square(0, 3);
    public static readonly Square A5 = new Square(0, 4);
    public static readonly Square A6 = new Square(0, 5);
    public static readonly Square A7 = new Square(0, 6);
    public static readonly Square A8 = new Square(0, 7);
    public static readonly Square B1 = new Square(1, 0);
    public static readonly Square B2 = new Square(1, 1);
    public static readonly Square B3 = new Square(1, 2);
    public static readonly Square B4 = new Square(1, 3);
    public static readonly Square B5 = new Square(1, 4);
    public static readonly Square B6 = new Square(1, 5);
    public static readonly Square B7 = new Square(1, 6);
    public static readonly Square B8 = new Square(1, 7);
    public static readonly Square C1 = new Square(2, 0);
    public static readonly Square C2 = new Square(2, 1);
    public static readonly Square C3 = new Square(2, 2);
    public static readonly Square C4 = new Square(2, 3);
    public static readonly Square C5 = new Square(2, 4);
    public static readonly Square C6 = new Square(2, 5);
    public static readonly Square C7 = new Square(2, 6);
    public static readonly Square C8 = new Square(2, 7);
    public static readonly Square D1 = new Square(3, 0);
    public static readonly Square D2 = new Square(3, 1);
    public static readonly Square D3 = new Square(3, 2);
    public static readonly Square D4 = new Square(3, 3);
    public static readonly Square D5 = new Square(3, 4);
    public static readonly Square D6 = new Square(3, 5);
    public static readonly Square D7 = new Square(3, 6);
    public static readonly Square D8 = new Square(3, 7);
    public static readonly Square E1 = new Square(4, 0);
    public static readonly Square E2 = new Square(4, 1);
    public static readonly Square E3 = new Square(4, 2);
    public static readonly Square E4 = new Square(4, 3);
    public static readonly Square E5 = new Square(4, 4);
    public static readonly Square E6 = new Square(4, 5);
    public static readonly Square E7 = new Square(4, 6);
    public static readonly Square E8 = new Square(4, 7);
    public static readonly Square F1 = new Square(5, 0);
    public static readonly Square F2 = new Square(5, 1);
    public static readonly Square F3 = new Square(5, 2);
    public static readonly Square F4 = new Square(5, 3);
    public static readonly Square F5 = new Square(5, 4);
    public static readonly Square F6 = new Square(5, 5);
    public static readonly Square F7 = new Square(5, 6);
    public static readonly Square F8 = new Square(5, 7);
    public static readonly Square G1 = new Square(6, 0);
    public static readonly Square G2 = new Square(6, 1);
    public static readonly Square G3 = new Square(6, 2);
    public static readonly Square G4 = new Square(6, 3);
    public static readonly Square G5 = new Square(6, 4);
    public static readonly Square G6 = new Square(6, 5);
    public static readonly Square G7 = new Square(6, 6);
    public static readonly Square G8 = new Square(6, 7);
    public static readonly Square H1 = new Square(7, 0);
    public static readonly Square H2 = new Square(7, 1);
    public static readonly Square H3 = new Square(7, 2);
    public static readonly Square H4 = new Square(7, 3);
    public static readonly Square H5 = new Square(7, 4);
    public static readonly Square H6 = new Square(7, 5);
    public static readonly Square H7 = new Square(7, 6);
    public static readonly Square H8 = new Square(7, 7);

    public static Square[] All =
    [
        A1, A2, A3, A4, A5, A6, A7, A8,
        B1, B2, B3, B4, B5, B6, B7, B8,
        C1, C2, C3, C4, C5, C6, C7, C8,
        D1, D2, D3, D4, D5, D6, D7, D8,
        E1, E2, E3, E4, E5, E6, E7, E8,
        F1, F2, F3, F4, F5, F6, F7, F8,
        G1, G2, G3, G4, G5, G6, G7, G8,
        H1, H2, H3, H4, H5, H6, H7, H8,
    ];
}
