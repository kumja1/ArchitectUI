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
    private struct Line
    {
        public string Text { get; set; }
        public Vector2 Position { get; set; }
    }

    private readonly List<Line> _lines = [];

    private int _currentLineY = 0;

    public string Text
    {
        get => GetProperty(nameof(Text), string.Empty)!;
        set => SetProperty(nameof(Text), value);
    }

    public Color TextColor
    {
        get => GetProperty<Color>(nameof(TextColor));
        set => SetProperty(nameof(TextColor), value);
    }

    public Size TextSize
    {
        get => GetProperty<Size>(nameof(TextSize));
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

    public TextBlock()
    {
        TextColor = Color.Black;
    }

    public override void Draw(Canvas canvas)
    {
        if (_lines.Count == 0)
            return;

        foreach (var line in _lines)
            canvas.DrawString(line.Text, Font, TextColor, Position.X, line.Position.Y, TextSize);
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
            _currentLineY = 0;
        }

        if (appendedText == string.Empty)
            return;

        if (appendedText == "\n")
        {
            _currentLineY++;
            return;
        }

        foreach (var line in EvaluteString(appendedText))
        {
            _lines.Add(
                new Line
                {
                    Text = line,
                    Position = new Vector2(Position.X, Position.Y + _currentLineY * Font.Height),
                }
            );

            _currentLineY++;
        }

        int newWidth = _lines.Max(l => l.Position.X) + Font.Width;
        int newHeight = _lines.Count * Font.Height;
        if (newWidth > Size.Width || newHeight > Size.Height)
            Size = new Size(newWidth, newHeight);
    }

    private IEnumerable<string> EvaluteString(string text)
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
}
