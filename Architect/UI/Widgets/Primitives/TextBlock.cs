using System.Drawing;
using Architect.UI.Widgets.Base;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

namespace Architect.UI.Widgets.Primitives;

class TextBlock : Widget
{
    public string Text
    {
        get => GetProperty<string>(nameof(Text));
        set => SetProperty(nameof(Text), value);
    }

    public Color TextColor
    {
        get => GetProperty<Color>(nameof(TextColor));
        set => SetProperty(nameof(TextColor), value);
    }

    public bool WrapText { get; set; }

    public Font Font { get; set; } = PCScreenFont.Default;


    public TextBlock()
    {
        TextColor = Color.Black;
    }

    public override void Draw(Canvas canvas)
    {

        if (WrapText)
        {
            var lines = WrapString(Text);
            var currentY = Position.Y;
            foreach (var line in lines)
            {
                canvas.DrawString(line, Font, TextColor, Position.X, currentY);
                currentY += Font.Height;
            }
        }
        else
        {
            canvas.DrawString(Text, Font, TextColor, Position.X, Position.Y);
        }
    }

    private List<string> WrapString(string text)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        var currentLine = string.Empty;
        foreach (var word in words)
        {
            if ((Font.Width * (word.Length + currentLine.Length)) > Size.Width)
            {
                lines.Add(currentLine);
                currentLine = word + " ";
            }
            else
            {
                currentLine += word + " ";
            }
        }
        if (string.IsNullOrWhiteSpace(currentLine) == false)
            lines.Add(currentLine);

        return lines;
    }

   
}

