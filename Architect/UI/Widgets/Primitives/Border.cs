using System.Drawing;
using Architect.Common.Utils;
using Size = Architect.Common.Models.Size;
using Cosmos.System.Graphics;
using Architect.UI.Drawing;
using Architect.UI.Base;

namespace Architect.UI.Primitives;

class Border : Widget
{

    public Color OutlineColor
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public Size OutlineThickness
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public int OutlineRadius
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public Border()
    {

        OutlineColor = ColorHelper.GetMonoChromaticColor(BackgroundColor ?? Color.Black)
        OutlineThickness = Size.Zero;
        OutlineRadius = 0;
        Size += OutlineThickness;
    }


    public override void Draw(Canvas canvas)
    {
        if (OutlineRadius == 0)
        {
            canvas.DrawRectangle(OutlineColor, Position.X, Position.Y, Size.Width, Size.Height);
            return;
        }
        else
            canvas.DrawRoundedRectangle(OutlineColor, Position.X, Position.Y, Size.Width, Size.Height, OutlineRadius);

        Content?.Draw(canvas);
    }
}