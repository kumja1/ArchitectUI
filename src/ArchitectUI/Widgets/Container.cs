using System.Drawing;

namespace Architect.Widgets;

class Container : Widget
{
    public override void Draw() => Context.Canvas.DrawRectangle(BackgroundColor, Position.X, Position.Y, Size.Width, Size.Height);

}