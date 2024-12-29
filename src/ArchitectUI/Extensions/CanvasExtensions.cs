using System.Drawing;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

static class CanvasExtensions
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
}
