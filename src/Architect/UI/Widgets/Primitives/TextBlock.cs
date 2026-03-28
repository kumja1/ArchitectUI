using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;

namespace Architect.UI.Widgets.Primitives;

public class TextBlock : Widget
{
    private readonly record struct Line(string Text, Vector2 Position);

    private readonly List<Line> _lines = [];

    private int _lineY;

    public string Text
    {
        get => GetProperty(ref field, string.Empty)!;
        set => SetProperty(ref field, value, propertyChanged: OnTextChanged);
    }

    public Color TextColor
    {
        get => GetProperty(ref field, initialValue: Color.Black);
        set => SetProperty(ref field, value);
    }

    public bool EnableTextWrapping
    {
        get => GetProperty(ref field);
        set => SetProperty(ref field, value);
    }

    public Font TextFont
    {
        get => GetProperty(ref field, PCScreenFont.DefaultFont)!;
        set => SetProperty(ref field, value);
    }

    public override void BeginDraw(Canvas canvas)
    {
        if (_lines.Count == 0)
            return;

        DrawBackground(canvas);
        Draw(canvas);
    }

    public override void Draw(Canvas canvas)
    {
        foreach (Line line in _lines)
            canvas.DrawString(
                line.Text,
                TextFont,
                TextColor,
                line.Position.X,
                line.Position.Y
            );
    }

    public override Size Measure(Size availableSize)
    {
        double width = 0;
        double height = 0;
        foreach (Line line in _lines)
        {
            width = Math.Max(width, line.Position.X + line.Text.Length * TextFont.Width);
            height += TextFont.Height;
        }

        return new Size(width, height) + Padding.Size;
    }

    private IEnumerable<string> WrapString(string text)
    {
        string[] words = text.Split(' ');
        string currentLine = string.Empty;
        foreach (string word in words)
        {
            if (TextFont.Width * (word.Length + currentLine.Length) > Width)
            {
                yield return currentLine;
                currentLine = word + " ";
            }
            else
                currentLine += word;
        }

        if (!string.IsNullOrWhiteSpace(currentLine))
            yield return currentLine;
    }

    private void OnTextChanged(string name, object currentValue, object value)
    {
        if (currentValue is not string oldString || value is not string newString)
            return;

        string appendedText =
            newString.Length > oldString.Length ? newString[oldString.Length..] : newString;

        if (newString.Length < oldString.Length) // Text was removed
        {
            int diff = oldString.Length - newString.Length;
            _lines.RemoveRange(oldString.Length - 1, diff);
            _lineY =  Math.Max(0, _lineY - TextFont.Height * diff);
        }

        foreach (string line in EvaluateString(appendedText))
        {
            _lines.Add(
                new Line
                {
                    Text = line,
                    Position = new Vector2(
                        X,
                        Y + _lineY
                    )
                }
            );

            _lineY++;
        }

        // int newWidth = _lines.Max(l => l.Position.X) + Font.Width;
        // int newHeight = _lines.Count * Font.Height;
        // if (newWidth > Size.Width || newHeight > Size.Height)
        //    Size = new Size(newWidth, newHeight);
    }

    private IEnumerable<string> EvaluateString(string text)
    {
        string[] lines = text.Split('\n');
        foreach (string line in lines)
        {
            string trimmedLine = line.TrimEnd();
            if (string.IsNullOrWhiteSpace(trimmedLine))
                continue;

            if (EnableTextWrapping)
                foreach (string wrappedLine in WrapString(line))
                    yield return wrappedLine;
            else
                yield return trimmedLine;
        }
    }
}