namespace Architect.Common.Models;

public readonly record struct Rect(Vector2 Position, Size Size)
{
    public Rect(float x, float y, float width, float height)
        : this(new Vector2(x, y), new Size(width, height)) { }

    public static Rect Zero => new(Vector2.Zero, Size.Zero);

    public static Rect Infinite => new(Vector2.Zero, Size.Infinite);

    public float Width => Size.Width;
    public float Height => Size.Height;

    public float X => Position.X;
    public float Y => Position.Y;

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
