using System.Drawing;
using Architect.Common.Interfaces;
using Architect.UI.Enums;
using Architect.UI.Widgets;
using Cosmos.System.Graphics.Fonts;

namespace Architect.UI;

class TextBlock : Widget
{
    public string Text
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public Color TextColor
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public bool WrapText { get; set; }

    public Font Font { get; set; } = PCScreenFont.Default;


    public TextBlock()
    {
        TextColor = Color.Black;
    }

    public override void Draw()
    {

        if (WrapText)
        {
            var lines = WrapString(Text);
            var currentY = Position.Y;
            foreach (var line in lines)
            {
                Context.Canvas.DrawString(line, Font, TextColor, Position.X, currentY);
                currentY += Font.Height;
            }
        }
        else
        {
            Context.Canvas.DrawString(Text, Font, TextColor, Position.X, Position.Y);
        }
    }

    private List<string> WrapString(string text)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        var currentLine = string.Empty;
        foreach (var word in words)
        {
            if ((Font.Width * (word.Length + currentLine.Length)) > Context.Size.Width)
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

