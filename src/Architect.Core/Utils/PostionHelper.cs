using Architect.Common.Models;

static class PositionHelper
{

    public static bool Contains(Vector2 position, Size size, int x, int y) =>
        x >= position.X && x <= position.X + size.Width && y >= position.Y && y <= position.Y + size.Height;
}