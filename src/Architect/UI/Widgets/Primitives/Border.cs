using System.Drawing;
using Architect.Utilities;
using Cosmos.Kernel.System.Graphics;

namespace Architect.UI.Widgets.Primitives;

public class Border : SingleContentWidget
{
    public Color OutlineColor
    {
        get =>
            GetProperty(
                ref field,
                initialValue: ColorHelper.GetMonoChromaticColor(BackgroundColor)
            );
        set => SetProperty(ref field, value);
    }

    public Size OutlineThickness
    {
        get => GetProperty(ref field, initialValue: Size.Zero);
        set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Gets or sets the radius of the border outline.
    /// </summary>
    public int OutlineRadius
    {
        get => GetProperty(ref field, initialValue: 0);
        set => SetProperty(ref field, value);
    }

    public override void Draw(Canvas canvas)
    {
        if (OutlineRadius > 0)
            canvas.DrawRectangle(
                OutlineColor,
                X,
                Y,
                Width + OutlineThickness.Width,
                Height + OutlineThickness.Height
                // OutlineRadius
            );
    }
}