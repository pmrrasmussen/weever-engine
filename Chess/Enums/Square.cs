namespace Chess.Enums;

// We use a classic 10x12 mailbox board representation
// https://www.chessprogramming.org/10x12_Board
public enum Square
{
    A8 = 91, B8 = 92, C8 = 93, D8 = 94, E8 = 95, F8 = 96, G8 = 97, H8 = 98,
    A7 = 81, B7 = 82, C7 = 83, D7 = 84, E7 = 85, F7 = 86, G7 = 87, H7 = 88,
    A6 = 71, B6 = 72, C6 = 73, D6 = 74, E6 = 75, F6 = 76, G6 = 77, H6 = 78,
    A5 = 61, B5 = 62, C5 = 63, D5 = 64, E5 = 65, F5 = 66, G5 = 67, H5 = 68,
    A4 = 51, B4 = 52, C4 = 53, D4 = 54, E4 = 55, F4 = 56, G4 = 57, H4 = 58,
    A3 = 41, B3 = 42, C3 = 43, D3 = 44, E3 = 45, F3 = 46, G3 = 47, H3 = 48,
    A2 = 31, B2 = 32, C2 = 33, D2 = 34, E2 = 35, F2 = 36, G2 = 37, H2 = 38,
    A1 = 21, B1 = 22, C1 = 23, D1 = 24, E1 = 25, F1 = 26, G1 = 27, H1 = 28,

    NullSquare = 0,
}

public static class SquareExtensions
{
    public static int Rank(this Square square)
        => (int)square / 10;

    public static Square ToSquare(this string square)
    {
        if (square.Length != 2)
            throw new ArgumentOutOfRangeException($"Unknown square {square}");

        var x = square[0] switch
        {
            'A' or 'a' => 20,
            'B' or 'b' => 30,
            'C' or 'c' => 40,
            'D' or 'd' => 50,
            'E' or 'e' => 60,
            'F' or 'f' => 70,
            'G' or 'g' => 80,
            'H' or 'h' => 90,
            _ => throw new ArgumentOutOfRangeException($"Unknown square {square}"),
        };

        if (!int.TryParse(square[1].ToString(), out var y) && y is > 0 and < 9)
            throw new ArgumentOutOfRangeException($"Unknown square {square}");

        return (Square)x + y;
    }

    public static string ToString(this Square square)
    {
        var column = ((int)square % 10) switch
        {
            1 => "A",
            2 => "B",
            3 => "C",
            4 => "D",
            5 => "E",
            6 => "F",
            7 => "G",
            8 => "H",
            _ => throw new ArgumentOutOfRangeException(),
        };

        var row = ((int)square/10 - 1).ToString();

        return column + row;
    }
}

public static class Squares
{
    public static Square[] All =
    [
        Square.A1, Square.A2, Square.A3, Square.A4, Square.A5, Square.A6, Square.A7, Square.A8,
        Square.B1, Square.B2, Square.B3, Square.B4, Square.B5, Square.B6, Square.B7, Square.B8,
        Square.C1, Square.C2, Square.C3, Square.C4, Square.C5, Square.C6, Square.C7, Square.C8,
        Square.D1, Square.D2, Square.D3, Square.D4, Square.D5, Square.D6, Square.D7, Square.D8,
        Square.E1, Square.E2, Square.E3, Square.E4, Square.E5, Square.E6, Square.E7, Square.E8,
        Square.F1, Square.F2, Square.F3, Square.F4, Square.F5, Square.F6, Square.F7, Square.F8,
        Square.G1, Square.G2, Square.G3, Square.G4, Square.G5, Square.G6, Square.G7, Square.G8,
        Square.H1, Square.H2, Square.H3, Square.H4, Square.H5, Square.H6, Square.H7, Square.H8,
    ];
}
