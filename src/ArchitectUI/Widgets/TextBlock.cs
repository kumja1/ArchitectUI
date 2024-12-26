using System.Drawing;
using Cosmos.System.Graphics.Fonts;

namespace Architect.Widgets;

class TextBlock : Widget
{
    public string Text { get; set; }

    public Color TextColor { get; set; } = Color.Black;

    public override void Draw() => Context.Canvas.DrawString(Text,Font., TextColor, Position.X, Position.Y);

}

