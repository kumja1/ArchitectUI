using System.Drawing;
using Architect.UI.Models;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using Size = Architect.UI.Models.Size;

namespace Architect.UI.Extensions;

public static class CanvasExtensions
{
    public static void DrawString(this Canvas canvas, string text, Font font, Color color, int x, int y, int width, int height)
    {
        var sizeX = (float)width / (font.Width * text.Length);
        var sizeY = height / font.Height;

        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            var charX = (int)(x + i * font.Width * sizeX);
            var charY = y + height - font.Height * sizeY / 2;

            canvas.DrawChar(c, font, color, charX, charY);
        }
    }

    public static void Clear(this Canvas canvas, Size size, Vector2 position) => Clear(canvas, size, position, Color.Black);

    public static void Clear(this Canvas canvas, Size size, Vector2 position, Color color)
    {
        for (var y = 0; y < size.Height; y++)
        {
            for (var x = 0; x < size.Width; x++)
            {
                canvas.DrawPoint(color, position.X + x, position.Y + y);
            }
        }
    }
}
