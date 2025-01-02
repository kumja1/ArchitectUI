namespace Architect.Common.Models;

public struct Vector2(int X, int Y)
{
    public int X { get; set; } = X;

    public int Y { get; set; } = Y;

    public static Vector2 Zero => new(0, 0);

    public static Vector2 Infinite => new(int.MaxValue, int.MaxValue);

    

    public static Vector2 operator *(Vector2 left, int right) => new(left.X * right, left.Y * right);

    public static Vector2 operator +(Vector2 left, Vector2 right) => new(left.X + right.X, left.Y + right.Y);

    public static Vector2 operator -(Vector2 left, Vector2 right) => new(left.X - right.X, left.Y - right.Y);

    public readonly bool Equals(Vector2 other) => X == other.X && Y == other.Y;

    public override bool Equals(object obj) => obj is Vector2 other && Equals(other);

    public override readonly int GetHashCode() => HashCode.Combine(X, Y);

    public static bool operator ==(Vector2 left, Vector2 right) => left.Equals(right);

    public static bool operator !=(Vector2 left, Vector2 right) => !left.Equals(right);
}
