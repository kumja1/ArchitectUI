using System.Drawing;
using Architect.Widgets;

namespace Architect.UI;

class Button : Widget
{
    public string Text { get => field; set => SetProperty(ref field, value); }
    public Color BackgroundColor { get => SetProperty(ref field, value); set => SetProperty(ref field, value); }

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
                WrapTexIt = false,
            }
        };
    }
}
