namespace Architect.Common.Models;

public readonly record struct Rect(Vector2 Position, Size Size)
{
    public Rect(double x, double y, double width, double height)
        : this(new Vector2(x, y), new Size(width, height)) { }

    public Rect(Vector2 position, double width, double height)
        : this(position, new Size(width, height)) { }

    public Rect(double x, double y, Size size)
        : this(new Vector2(x, y), size) { }

    public static Rect Zero => new(Vector2.Zero, Size.Zero);

    public static Rect Infinite => new(Vector2.Zero, Size.Infinite);

    public double Width => Size.Width;
    public double Height => Size.Height;

    public double X => Position.X;
    public double Y => Position.Y;

    public static Rect operator *(Rect left, int right) =>
        new(left.Position * right, left.Size * right);

    public static Rect operator +(Rect left, Vector2 right) =>
        new(left.Position + right, left.Size);

    public static Rect operator -(Rect left, Vector2 right) =>
        new(left.Position - right, left.Size);

    public static Rect operator +(Rect left, Rect right) =>
        new(left.Position + right.Position, left.Size + right.Size);

    public static Rect operator -(Rect left, Rect right) =>
        new(left.Position - right.Position, left.Size - right.Size);

    public override readonly int GetHashCode() => HashCode.Combine(Position, Size);
}
