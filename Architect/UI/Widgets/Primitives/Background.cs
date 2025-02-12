using System.Drawing;
using Architect.Core.Rendering;
using Architect.UI;
using Cosmos.System.Graphics;

namespace Architect.UI;

class Background : Widget
{
    public Color Color
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public override void Draw(Canvas canvas)
    {
        canvas.DrawFilledRectangle(Color, Position.X, Position.Y, Size.Width, Size.Height);
        Content.Draw(canvas);
    }
}