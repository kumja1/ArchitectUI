using System.Drawing;
using Size = Architect.Common.Models.Size;
using Architect.UI.Utils;
using Architect.UI;
using Architect.UI.Extensions;

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
        OutlineColor = ColorHelper.GetMonoChromaticColor(BackgroundColor);
        OutlineThickness = Size.Zero;
        OutlineRadius = 0;
        Size += OutlineThickness;
    }

    public override void Draw()
    {
        if (OutlineRadius == 0)
        {
            Context.Canvas.DrawRectangle(OutlineColor, Position.X, Position.Y, Size.Width, Size.Height);
            return;
        }
        else
        {
            Context.Canvas.DrawRoundedRectangle(OutlineColor, Position.X, Position.Y, Size.Width, Size.Height, OutlineRadius);
        }

        Content?.Draw();
    }
}