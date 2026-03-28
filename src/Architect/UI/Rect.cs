namespace Architect.UI;

public readonly record struct Rect(Vector2 Position, Size Size)
{
    public Rect(int x, int y, int width, int height)
        : this(new Vector2(x, y), new Size(width, height))
    {
    }

    public Rect(Vector2 position, int width, int height)
        : this(position, new Size(width, height))
    {
    }

    public Rect(int x, int y, Size size)
        : this(new Vector2(x, y), size)
    {
    }

    public double Width => Size.Width;
    public double Height => Size.Height;

    public double X => Position.X;
    public double Y => Position.Y;

    public static readonly Rect Zero = new(Vector2.Zero, Size.Zero);

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
    
    public override int GetHashCode() => HashCode.Combine(Position, Size);

    public bool Within(Rect other)
    {
        return X > other.X && X < other.X + other.Width &&
               Y > other.Y && Y < other.Y + other.Height;
    }
}