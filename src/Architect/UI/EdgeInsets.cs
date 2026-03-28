namespace Architect.UI;

public readonly record struct EdgeInsets(float Left, float Top, float Right, float Bottom)
{
    public readonly Size Size = new(Left + Right, Top + Bottom);

    public double Width => Size.Width;

    public double Height => Size.Height;

    public static readonly EdgeInsets Zero = FromAll(0);

    public static EdgeInsets operator +(EdgeInsets a, EdgeInsets b) =>
        new(a.Left + b.Left, a.Top + b.Top, a.Right + b.Right, a.Bottom + b.Bottom);

    public static EdgeInsets operator -(EdgeInsets a, EdgeInsets b) =>
        new(a.Left - b.Left, a.Top - b.Top, a.Right - b.Right, a.Bottom - b.Bottom);

    public static EdgeInsets operator *(EdgeInsets a, float b) =>
        new(a.Left * b, a.Top * b, a.Right * b, a.Bottom * b);

    public static EdgeInsets FromSide(float left = 0, float top = 0, float right = 0, float bottom = 0) =>
        new(left, top, right, bottom);

    public static EdgeInsets FromAxis(float horizontal = 0, float vertical = 0) =>
        new(horizontal, vertical, horizontal, vertical);

    public static EdgeInsets FromAll(float uniform) => new(uniform, uniform, uniform, uniform);
}