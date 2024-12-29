using System.Drawing;
using Cosmos.System.Graphics.Fonts;

namespace Architect.Widgets;

class TextBlock : Widget
{
    public string Text
    {
        get => field;
        set
        {
            if (ShouldRedraw(Text, value))
            {
                field = value;
                Draw();
            }
        }
    }

    public Color TextColor
    {
        get => field;
        set
        {
            if (ShouldRedraw(TextColor, value))
            {
                field = value;
                Draw();
            }
        }
    } = Color.Black;

    public bool WrapText { get; set; } = false;


    public Font Font { get; set; } = PCScreenFont.Default;



    public override void Draw()
    {
    
        if (WrapText)
        {
            Context.Canvas.DrawString(Text, Font, TextColor, Position.X, Position.Y, Context.Size.Width, Context.Size.Height);
        }
        else
        {
            Context.Canvas.DrawString(Text, Font, TextColor, Position.X, Position.Y);
        }
    }

}

