using System.Drawing;
using Architect.Common.Models;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using Size = Architect.Common.Models.Size;

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


    public static void Clear(this Canvas canvas, Size size, Vector2 position, Color? color = null)
    {
        color ??= Color.Black;  
        for (var y = 0; y < size.Height; y++)
        {
            for (var x = 0; x < size.Width; x++)
            {
                canvas.DrawPoint((Color)color, position.X + x, position.Y + y);
            }
        }
    }


     public static void DrawRoundedRectangle(
        this Canvas canvas,
        Color color,
        int x,
        int y,
        int width,
        int height,
        int cornerRadius)
    {
        cornerRadius = Math.Min(cornerRadius, Math.Min(width / 2, height / 2));

        canvas.DrawArc(x, y, cornerRadius * 2, cornerRadius * 2,color, 180, 90); 
        canvas.DrawArc(x + width - cornerRadius * 2, y, cornerRadius * 2, cornerRadius * 2, color,270, 90);
        canvas.DrawArc(x, y + height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, color,90, 90); 
        canvas.DrawArc(x + width - cornerRadius * 2, y + height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2,color, 0, 90); 

        canvas.DrawFilledRectangle(color, x + cornerRadius, y, width - 2 * cornerRadius, cornerRadius); 
        canvas.DrawFilledRectangle(color, x + cornerRadius, y + height - cornerRadius, width - 2 * cornerRadius, cornerRadius);
        canvas.DrawFilledRectangle(color, x, y + cornerRadius, cornerRadius, height - 2 * cornerRadius); 
        canvas.DrawFilledRectangle(color, x + width - cornerRadius, y + cornerRadius, cornerRadius, height - 2 * cornerRadius); 

        canvas.DrawFilledRectangle(color, x + cornerRadius, y + cornerRadius, width - 2 * cornerRadius, height - 2 * cornerRadius);
    }
}
