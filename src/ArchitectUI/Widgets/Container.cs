using System.Drawing;

namespace Architect.Widgets;

class Container : Widget
{

    public Container()
    {
        BackgroundColor = Color.White;
    }

    public override void Draw() => Context.Canvas.DrawRectangle(BackgroundColor, Position.X, Position.Y, Size.Width, Size.Height);
    
}