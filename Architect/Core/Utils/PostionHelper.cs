using Architect.Common.Models;

static class PositionHelper
{

    public static bool PositionWithin(Vector2 position, Size size, int x, int y) =>
        x >= position.X && x <= position.X + size.Width && y >= position.Y && y <= position.Y + size.Height;

    public static bool PositionWithin(Vector2 position, Size size, Vector2 point) => PositionWithin(position, size, point.X, point.Y);
}