using System.Drawing;
using Architect.UI.Extensions;
using Architect.Common.Utils;
using Size = Architect.Common.Models.Size;

namespace Architect.UI;

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
        OutlineColor = IsAncestor<Background>() ? ColorHelper.GetMonoChromaticColor(GetAncestor<Background>().Color) : Color.Black;
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