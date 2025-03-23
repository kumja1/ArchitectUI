using System.Drawing;
using Architect.Common.Utilities;
using Architect.UI.Drawing;
using Architect.UI.Widgets.Base;
using Cosmos.System.Graphics;
using Size = Architect.Common.Models.Size;

namespace Architect.UI.Widgets.Primitives;

class Border : Widget
{
    public Color OutlineColor
    {
        get =>
            GetProperty(
                nameof(OutlineColor),
                defaultValue: ColorHelper.GetMonoChromaticColor(BackgroundColor)
            );
        set => SetProperty(nameof(OutlineColor), value);
    }

    public Size OutlineThickness
    {
        get => GetProperty<Size>(nameof(OutlineThickness), defaultValue: Size.Zero);
        set => SetProperty(nameof(OutlineThickness), value);
    }

    public int OutlineRadius
    {
        get => GetProperty(nameof(OutlineRadius), defaultValue: 0);
        set => SetProperty(nameof(OutlineRadius), value);
    }

    public override void Draw(Canvas canvas)
    {
        if (OutlineRadius > 0)
            canvas.DrawRoundedRectangle(
                OutlineColor,
                Position.X,
                Position.Y,
                Size + OutlineThickness,
                OutlineRadius
            );

        base.Draw(canvas);
    }
}
