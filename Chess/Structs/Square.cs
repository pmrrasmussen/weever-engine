namespace Chess.Structs;

public readonly struct Square(int x, int y)
{
    public readonly int X = x;

    public readonly int Y = y;

    public static Square operator +(Square square, Vector vector)
    {
        return new Square(square.X + vector.X, square.Y + vector.Y);
    }
}

public readonly struct Vector(int x, int y)
{
    public readonly int X = x;

    public readonly int Y = y;
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
    public static Square A1 = new Square(0, 0);
    public static Square A2 = new Square(1, 0);
    public static Square A3 = new Square(2, 0);
    public static Square A4 = new Square(3, 0);
    public static Square A5 = new Square(4, 0);
    public static Square A6 = new Square(5, 0);
    public static Square A7 = new Square(6, 0);
    public static Square A8 = new Square(7, 0);
    public static Square B1 = new Square(0, 1);
    public static Square B2 = new Square(1, 1);
    public static Square B3 = new Square(2, 1);
    public static Square B4 = new Square(3, 1);
    public static Square B5 = new Square(4, 1);
    public static Square B6 = new Square(5, 1);
    public static Square B7 = new Square(6, 1);
    public static Square B8 = new Square(7, 1);
    public static Square C1 = new Square(0, 2);
    public static Square C2 = new Square(1, 2);
    public static Square C3 = new Square(2, 2);
    public static Square C4 = new Square(3, 2);
    public static Square C5 = new Square(4, 2);
    public static Square C6 = new Square(5, 2);
    public static Square C7 = new Square(6, 2);
    public static Square C8 = new Square(7, 2);
    public static Square D1 = new Square(0, 3);
    public static Square D2 = new Square(1, 3);
    public static Square D3 = new Square(2, 3);
    public static Square D4 = new Square(3, 3);
    public static Square D5 = new Square(4, 3);
    public static Square D6 = new Square(5, 3);
    public static Square D7 = new Square(6, 3);
    public static Square D8 = new Square(7, 3);
    public static Square E1 = new Square(0, 4);
    public static Square E2 = new Square(1, 4);
    public static Square E3 = new Square(2, 4);
    public static Square E4 = new Square(3, 4);
    public static Square E5 = new Square(4, 4);
    public static Square E6 = new Square(5, 4);
    public static Square E7 = new Square(6, 4);
    public static Square E8 = new Square(7, 4);
    public static Square F1 = new Square(0, 5);
    public static Square F2 = new Square(1, 5);
    public static Square F3 = new Square(2, 5);
    public static Square F4 = new Square(3, 5);
    public static Square F5 = new Square(4, 5);
    public static Square F6 = new Square(5, 5);
    public static Square F7 = new Square(6, 5);
    public static Square F8 = new Square(7, 5);
    public static Square G1 = new Square(0, 6);
    public static Square G2 = new Square(1, 6);
    public static Square G3 = new Square(2, 6);
    public static Square G4 = new Square(3, 6);
    public static Square G5 = new Square(4, 6);
    public static Square G6 = new Square(5, 6);
    public static Square G7 = new Square(6, 6);
    public static Square G8 = new Square(7, 6);
    public static Square H1 = new Square(0, 7);
    public static Square H2 = new Square(1, 7);
    public static Square H3 = new Square(2, 7);
    public static Square H4 = new Square(3, 7);
    public static Square H5 = new Square(4, 7);
    public static Square H6 = new Square(5, 7);
    public static Square H7 = new Square(6, 7);
    public static Square H8 = new Square(7, 7);
}
