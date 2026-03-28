namespace Architect.UI;

public readonly record struct Vector2(int X, int Y)
{
    public static readonly Vector2 Zero = new(0, 0);

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

    public static bool Within(Vector2 value, Vector2 min, Vector2 max) =>
        Within(min.X, min.Y, max.X, max.Y, value);

    public static bool Within(double minX, double minY, double maxX, double maxY, Vector2 value) =>
        value.X >= minX && value.X <= maxX && value.Y >= minY && value.Y <= maxY;

    public bool Within(Vector2 min, Vector2 max) => Within(this, min, max);

    public bool Within(double minX, double minY, double maxX, double maxY) =>
        Within(minX, minY, maxX, maxY, this);

    public Vector2 Clamp(Vector2 min, Vector2 max) => new(Math.Clamp(X, min.X, max.X), Math.Clamp(Y, min.Y, max.Y));


    public override int GetHashCode() => HashCode.Combine(X, Y);
}