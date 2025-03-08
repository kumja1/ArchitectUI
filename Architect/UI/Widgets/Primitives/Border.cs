using System.Drawing;
using Architect.Common.Utilities;
using Size = Architect.Common.Models.Size;
using Cosmos.System.Graphics;
using Architect.UI.Drawing;
using Architect.UI.Widgets.Base;

namespace Architect.UI.Widgets.Primitives;

class Border : Widget
{

    public Color OutlineColor
    {
        get => GetProperty<Color>(nameof(OutlineColor));
        set => SetProperty(nameof(OutlineColor), value);
    }

    public Size OutlineThickness
    {
        get => GetProperty<Size>(nameof(OutlineThickness));
        set => SetProperty(nameof(OutlineThickness), value);
    }

    public int OutlineRadius
    {
        get => GetProperty<int>(nameof(OutlineRadius));
        set => SetProperty(nameof(OutlineRadius), value);
    }

    public Border()
    {
        OutlineColor = ColorHelper.GetMonoChromaticColor(BackgroundColor);
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