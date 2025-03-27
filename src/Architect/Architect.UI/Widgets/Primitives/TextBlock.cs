using System.Drawing;
using Architect.Common.Models;
using Architect.UI.Drawing;
using Architect.UI.Widgets.Base;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using Size = Architect.Common.Models.Size;

namespace Architect.UI.Widgets.Primitives;

class TextBlock : Widget
{
    private readonly struct Line
    {
        public string Text { get; init; }

        public Vector2 Position { get; init; }

        public readonly int Length => Text.Length;
    }

    private readonly List<Line> _lines = [];

    private int _currentLineIndex = 0;

    public string Text
    {
        get => GetProperty(nameof(Text), string.Empty)!;
        set => SetProperty(nameof(Text), value);
    }

    public Color TextColor
    {
        get => GetProperty(nameof(TextColor), defaultValue: Color.Black);
        set => SetProperty(nameof(TextColor), value);
    }

    public Size TextSize
    {
        get => GetProperty(nameof(TextSize), defaultValue: Size.Zero);
        set => SetProperty(nameof(TextSize), value);
    }

    public bool EnableTextWrapping
    {
        get => GetProperty<bool>(nameof(EnableTextWrapping));
        set => SetProperty(nameof(EnableTextWrapping), value);
    }

    public Font Font
    {
        get => GetProperty<Font>(nameof(Font), PCScreenFont.Default)!;
        set => SetProperty(nameof(Font), value);
    }

    public override void Draw(Canvas canvas)
    {
        if (_lines.Count == 0)
            return;

        foreach (var line in _lines)
            canvas.DrawString(line.Text, Font, TextColor, Position.X, line.Position.Y, TextSize);

        base.Draw(canvas);
    }

    private IEnumerable<string> WrapString(string text)
    {
        var words = text.Split(' ');
        var currentLine = string.Empty;
        foreach (var word in words)
        {
            if ((Font.Width * (word.Length + currentLine.Length)) > Size.Width)
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

    protected override void OnPropertyChanged(string name, object currentValue, object value)
    {
        base.OnPropertyChanged(name, currentValue, value);

        if (name != nameof(Text))
            return;

        if (currentValue is not string oldString || value is not string newString)
            return;

        var appendedText =
            newString.Length > oldString.Length ? newString[oldString.Length..] : newString;

        if (newString.Length < oldString.Length) // Text was removed
        {
            _lines.Clear();
            _currentLineIndex = 0;
        }

        if (appendedText == string.Empty)
            return;

        if (appendedText == "\n")
        {
            _currentLineIndex++;
            return;
        }

        foreach (var line in EvaluateString(appendedText))
        {
            _lines.Add(
                new Line
                {
                    Text = line,
                    Position = new Vector2(
                        Position.X,
                        Position.Y - _currentLineIndex * Font.Height
                    ),
                }
            );

            _currentLineIndex++;
        }

        // int newWidth = _lines.Max(l => l.Position.X) + Font.Width;
        // int newHeight = _lines.Count * Font.Height;
        // if (newWidth > Size.Width || newHeight > Size.Height)
        //    Size = new Size(newWidth, newHeight);
    }

    private IEnumerable<string> EvaluateString(string text)
    {
        var lines = text.Split('\n');
        foreach (var line in lines)
        {
            var trimmedLine = line.TrimEnd();
            if (string.IsNullOrWhiteSpace(trimmedLine))
                continue;

            if (IsWrappable(trimmedLine))
                foreach (var wrappedLine in WrapString(line))
                    yield return wrappedLine;
            else
                yield return trimmedLine;
        }
    }

    private bool IsWrappable(string text) =>
        EnableTextWrapping && Font.Width * text.Length > Size.Width;

    public override Size GetNaturalSize() =>
        base.GetNaturalSize()
        + new Size(_lines.Max(l => l.Length) * Font.Width, _lines.Count * Font.Height);
}
