namespace Architect.Common.Models;

public struct Rect(Vector2 position, Size size)
{
    public Vector2 Position { get; set; } = position;

    public Size Size { get; set; } = size;

    public static Rect Zero => new(Vector2.Zero, Size.Zero);

    public static Rect Infinite => new(Vector2.Zero, Size.Infinite);

    public static Rect operator *(Rect left, int right) =>
        new(left.Position * right, left.Size * right);

    public static Rect operator +(Rect left, Rect right) =>
        new(left.Position + right.Position, left.Size + right.Size);

    public static Rect operator -(Rect left, Rect right) =>
        new(left.Position - right.Position, left.Size - right.Size);

    public readonly bool Equals(Rect other) => Position == other.Position && Size == other.Size;

    public override bool Equals(object obj) => obj is Rect other && Equals(other);

    public override readonly int GetHashCode() => HashCode.Combine(Position, Size);

    public static bool operator ==(Rect left, Rect right) => left.Equals(right);

    public static bool operator !=(Rect left, Rect right) => !left.Equals(right);

    public static Rect Clamp(Rect value, Rect min, Rect max) =>
        new(
            Vector2.Clamp(value.Position, min.Position, max.Position),
            Size.Clamp(value.Size, min.Size, max.Size)
        );
}
