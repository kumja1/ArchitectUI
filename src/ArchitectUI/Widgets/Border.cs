
using System.Drawing;
using Architect.Utils;
using Architect.Widgets;

class Border : Widget
{

    public Color OutlineColor
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            Draw();
        }
    }

    public int OutlineThickness
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            Draw();
        }
    }

    public Border()
    {
        OutlineColor = ColorHelper.GetMonoChromaticColor(BackgroundColor);
        OutlineThickness = 1;
    }

    public override void Draw()
    {
        Context.Canvas.DrawRectangle(OutlineColor, Position.X, Position.Y, Size.Width, Size.Height);
        Context.Canvas.DrawRectangle(BackgroundColor, Position.X + OutlineThickness, Position.Y + OutlineThickness, Size.Width - OutlineThickness * 2, Size.Height - OutlineThickness * 2);
    }
}