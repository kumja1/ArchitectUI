namespace Architect.UI;

public readonly record struct Size(int Width, int Height)
{
    public static readonly Size Zero = new(0, 0);

    public static Size operator *(Size left, int right) =>
        new(left.Width * right, left.Height * right);

    public static Size operator /(Size left, int right) =>
        new(left.Width / right, left.Height / right);

    public static Size operator +(Size left, Size right) =>
        new(left.Width + right.Width, left.Height + right.Height);

    public static Size operator -(Size left, Size right) =>
        new(left.Width - right.Width, left.Height - right.Height);

    public static Size Clamp(Size size, Size min, Size max)
        => new(Math.Clamp(size.Width, min.Width, max.Width), Math.Clamp(size.Height, min.Height, max.Height));
}