namespace Architect.UI;

public readonly record struct EdgeInsets(int Left, int Top, int Right, int Bottom)
{
    public readonly Size Size = new(Left + Right, Top + Bottom);

    public double Width => Size.Width;

    public double Height => Size.Height;

    public static readonly EdgeInsets Zero = FromAll(0);

    public static EdgeInsets operator +(EdgeInsets a, EdgeInsets b) =>
        new(a.Left + b.Left, a.Top + b.Top, a.Right + b.Right, a.Bottom + b.Bottom);

    public static EdgeInsets operator -(EdgeInsets a, EdgeInsets b) =>
        new(a.Left - b.Left, a.Top - b.Top, a.Right - b.Right, a.Bottom - b.Bottom);

    public static EdgeInsets operator *(EdgeInsets a, int b) =>
        new(a.Left * b, a.Top * b, a.Right * b, a.Bottom * b);

    public static EdgeInsets FromSide(int left = 0, int top = 0, int right = 0, int bottom = 0) =>
        new(left, top, right, bottom);

    public static EdgeInsets FromAxis(int horizontal = 0, int vertical = 0) =>
        new(horizontal, vertical, horizontal, vertical);

    public static EdgeInsets FromAll(int uniform) => new(uniform, uniform, uniform, uniform);
}