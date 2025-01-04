using System.Drawing;
using Architect.Widgets;

namespace Architect.UI.Widgets;


class Button : Widget
{
    public string Text { get; set; }

    public Button()
    {
        BackgroundColor = Color.White;
        Content = new Background
        {
            Color = BackgroundColor,
            Content = new TextBlock
            {
                Text = Text,
                TextColor = Color.Black,
                WrapText = false,
            }
        };
    }
}
