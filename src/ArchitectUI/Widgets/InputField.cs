using System.Drawing;
using Architect.Enums;

namespace Architect.Widgets;

class InputField : Widget
{
    public string Text
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            Draw();
        }
    }

    public InputField()
    {
        Content = new Border
        {
            OutlineColor = Color.White,
            OutlineThickness = 3,
            Content = new Container
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = new TextBlock
                {
                    Text = Text,
                    TextColor = Color.Black
                }
            }
        };
    }

    public override void Draw() => Content.Draw();

}



