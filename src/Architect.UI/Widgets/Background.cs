using System.Drawing;

namespace Architect.UI;

class Background : Widget
{
    public Color Color
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public override void Draw()
    {
        Context.Canvas.DrawFilledRectangle(Color, Position.X, Position.Y, Size.Width, Size.Height);
        Content.Draw();
    }
}