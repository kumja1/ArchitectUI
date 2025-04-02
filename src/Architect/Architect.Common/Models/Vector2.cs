namespace Architect.Common.Models;

public struct Vector2(double x, double y)
{
    public double X = x;
    public double Y = y;
    public static Vector2 Zero => new(0, 0);

    public static Vector2 Infinite => new(int.MaxValue, int.MaxValue);

    public static Vector2 operator *(Vector2 left, int right) =>
        new(left.X * right, left.Y * right);

    public static Vector2 operator +(Vector2 left, Vector2 right) =>
        new(left.X + right.X, left.Y + right.Y);

    public static Vector2 operator -(Vector2 left, Vector2 right) =>
        new(left.X - right.X, left.Y - right.Y);

    public static Vector2 operator /(Vector2 left, int right) =>
        new(left.X / right, left.Y / right);

    public static Vector2 operator -(Vector2 value) => new(-value.X, -value.Y);

    public static Vector2 operator ++(Vector2 value) => new(value.X + 1, value.Y + 1);

    public static Vector2 operator --(Vector2 value) => new(value.X - 1, value.Y - 1);

    public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max) =>
        new(Math.Clamp(value.X, min.X, max.X), Math.Clamp(value.Y, min.Y, max.Y));

    public static bool Within(Vector2 value, Vector2 min, Vector2 max) =>
        Within(min.X, min.Y, max.X, max.Y, value);

    public static bool Within(double minX, double minY, double maxX, double maxY, Vector2 value) =>
        value.X >= minX && value.X <= maxX && value.Y >= minY && value.Y <= maxY;

    public Vector2 Clamp(Vector2 min, Vector2 max) => Clamp(this, min, max);

    public bool Within(Vector2 min, Vector2 max) => Within(this, min, max);

    public bool Within(double minX, double minY, double maxX, double maxY) =>
        Within(minX, minY, maxX, maxY, this);

    public override readonly int GetHashCode() => HashCode.Combine(X, Y);
}
