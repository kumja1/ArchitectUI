using System.Drawing;
using Architect.Common.Models;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using Size = Architect.Common.Models.Size;

namespace Architect.UI.Drawing;

public static class CanvasEx
{
    public static void DrawString(
        this Canvas canvas,
        string text,
        Font font,
        Color color,
        int x,
        int y,
        Size size
    )
    {
        var width = size.Width;
        var height = size.Height;

        var sizeX = (float)width / (font.Width * text.Length);
        var sizeY = height / font.Height;

        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            var charX = (int)(x + i * font.Width * sizeX);
            var charY = (int)(y + height - font.Height * sizeY / 2);

            canvas.DrawChar(c, font, color, charX, charY);
        }
    }

    public static void DrawString(
        this Canvas canvas,
        string text,
        Font font,
        Color color,
        Vector2 position,
        Size size
    ) =>
        canvas.DrawString(
            text,
            font,
            color,
            (int)(position.X + size.Width),
            (int)(position.Y + size.Height)
        );

    public static void DrawRectangle(this Canvas canvas, Color color, int x, int y, Size size) =>
        DrawRectangle(canvas, color, x, y, size.Width, size.Height);

    public static void DrawRectangle(
        this Canvas canvas,
        Color color,
        double x,
        double y,
        double width,
        double height
    ) => canvas.DrawRectangle(color, x, y, (int)width, (int)height);

    public static void DrawRoundedRectangle(
        this Canvas canvas,
        Color color,
        int x,
        int y,
        int width,
        int height,
        int cornerRadius
    )
    {
        cornerRadius = Math.Min(cornerRadius, Math.Min(width / 2, height / 2));

        canvas.DrawArc(x, y, cornerRadius * 2, cornerRadius * 2, color, 180, 90);
        canvas.DrawArc(
            x + width - cornerRadius * 2,
            y,
            cornerRadius * 2,
            cornerRadius * 2,
            color,
            270,
            90
        );
        canvas.DrawArc(
            x,
            y + height - cornerRadius * 2,
            cornerRadius * 2,
            cornerRadius * 2,
            color,
            90,
            90
        );
        canvas.DrawArc(
            x + width - cornerRadius * 2,
            y + height - cornerRadius * 2,
            cornerRadius * 2,
            cornerRadius * 2,
            color,
            0,
            90
        );

        canvas.DrawFilledRectangle(
            color,
            x + cornerRadius,
            y,
            width - 2 * cornerRadius,
            cornerRadius
        );
        canvas.DrawFilledRectangle(
            color,
            x + cornerRadius,
            y + height - cornerRadius,
            width - 2 * cornerRadius,
            cornerRadius
        );
        canvas.DrawFilledRectangle(
            color,
            x,
            y + cornerRadius,
            cornerRadius,
            height - 2 * cornerRadius
        );
        canvas.DrawFilledRectangle(
            color,
            x + width - cornerRadius,
            y + cornerRadius,
            cornerRadius,
            height - 2 * cornerRadius
        );

        canvas.DrawFilledRectangle(
            color,
            x + cornerRadius,
            y + cornerRadius,
            width - 2 * cornerRadius,
            height - 2 * cornerRadius
        );
    }

    public static void DrawRoundedRectangle(
        this Canvas canvas,
        Color color,
        int x,
        int y,
        Size size,
        int cornerRadius
    ) => DrawRoundedRectangle(canvas, color, x, y, size.Width, size.Height, cornerRadius);

    public static void DrawRoundedRectangle(
        this Canvas canvas,
        Color color,
        double x,
        double y,
        double width,
        double height,
        int cornerRadius
    ) => canvas.DrawRoundedRectangle(color, x, y, width, height, cornerRadius);
}
