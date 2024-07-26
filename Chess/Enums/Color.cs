namespace Chess.Enums;

public enum Color
{
    White,
    Black,
}

public static class ColorExtensions
{
    public static Color Flip(this Color color)
        => color is Color.White ? Color.Black : Color.White;
}
