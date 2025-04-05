namespace Architect.Common.Models;

public readonly record struct EdgeInsets(float Left, float Top, float Right, float Bottom)
{
    public EdgeInsets(float uniform)
        : this(uniform, uniform, uniform, uniform) { }

    public readonly Size Size => new(Left + Right, Top + Bottom);

    public readonly double Width => Size.Width;

    public readonly double Height => Size.Height;

    public static EdgeInsets Zero => new(0);

    public static implicit operator EdgeInsets(float uniform) => new(uniform);

    public static EdgeInsets operator +(EdgeInsets a, EdgeInsets b) =>
        new(a.Left + b.Left, a.Top + b.Top, a.Right + b.Right, a.Bottom + b.Bottom);

    public static EdgeInsets operator -(EdgeInsets a, EdgeInsets b) =>
        new(a.Left - b.Left, a.Top - b.Top, a.Right - b.Right, a.Bottom - b.Bottom);

    public static EdgeInsets operator *(EdgeInsets a, float b) =>
        new(a.Left * b, a.Top * b, a.Right * b, a.Bottom * b);

    public static EdgeInsets FromTopLeft(float top, float left) => new(left, top, 0, 0);

    public static EdgeInsets FromTopRight(float top, float right) => new(0, top, right, 0);

    public static EdgeInsets FromBottomLeft(float bottom, float left) => new(left, 0, 0, bottom);

    public static EdgeInsets FromBottomRight(float bottom, float right) => new(0, 0, right, bottom);

    public static EdgeInsets FromTopBottom(float top, float bottom) => new(0, top, 0, bottom);

    public static EdgeInsets FromLeftRight(float left, float right) => new(left, 0, right, 0);

    public static EdgeInsets FromLeft(float left) => new(left, 0, 0, 0);

    public static EdgeInsets FromRight(float right) => new(0, 0, right, 0);

    public static EdgeInsets FromTop(float top) => new(0, top, 0, 0);

    public static EdgeInsets FromBottom(float bottom) => new(0, 0, 0, bottom);
}
