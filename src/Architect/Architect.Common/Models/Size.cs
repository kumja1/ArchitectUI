namespace Architect.Common.Models;

public readonly record struct Size(double width, double height)
{
    public readonly double Width = width;

    public readonly double Height = height;

    public static Size Zero => new(0, 0);

    public static Size Infinite => new(int.MaxValue, int.MaxValue);

    public static Size operator *(Size left, int right) =>
        new(left.Width * right, left.Height * right);

    public static Size operator /(Size left, int right) =>
        new(left.Width / right, left.Height / right);

    public static Size operator +(Size left, Size right) =>
        new(left.Width + right.Width, left.Height + right.Height);

    public static Size operator -(Size left, Size right) =>
        new(left.Width - right.Width, left.Height - right.Height);

    public static Size Clamp(Size value, Size min, Size max) =>
        new(
            Math.Clamp(value.Width, min.Width, max.Width),
            Math.Clamp(value.Height, min.Height, max.Height)
        );
}
