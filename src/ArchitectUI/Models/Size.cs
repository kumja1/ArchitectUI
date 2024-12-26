
struct Size(int width, int height)
{
    public int Width { get; set; } = width;

    public int Height { get; set; } = height;

    public static Size Zero => new(0, 0);

    public static Size Infinite => new(int.MaxValue, int.MaxValue);


    public static Size operator *(Size left, int right) => new(left.Width * right, left.Height * right);

    public static Size operator +(Size left, Size right) => new(left.Width + right.Width, left.Height + right.Height);

    public static Size operator -(Size left, Size right) => new(left.Width - right.Width, left.Height - right.Height);

    public readonly bool Equals(Size other) => Width == other.Width && Height == other.Height;
    public override bool Equals(object obj) => obj is Size other && Equals(other);
    public override readonly int GetHashCode() => HashCode.Combine(Width, Height);

    public static bool operator ==(Size left, Size right) => left.Equals(right);

    public static bool operator !=(Size left, Size right) => !left.Equals(right);

    public static Size Clamp(Size value, Size min, Size max) => new(
        Math.Clamp(value.Width, min.Width, max.Width),
        Math.Clamp(value.Height, min.Height, max.Height)
    );


}