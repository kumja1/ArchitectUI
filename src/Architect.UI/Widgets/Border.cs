using System.Drawing;
using Architect.UI.Utils;
using Architect.UI;

class Border : Widget
{

    public Color OutlineColor
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public int OutlineThickness
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public Border()
    {
        OutlineColor = ColorHelper.GetMonoChromaticColor(BackgroundColor);
        OutlineThickness = 1;
    }

    public override void Draw()
    {
        Context.Canvas.DrawRectangle(OutlineColor, Position.X, Position.Y, Size.Width, Size.Height);
        Content?.Draw();
    }
}