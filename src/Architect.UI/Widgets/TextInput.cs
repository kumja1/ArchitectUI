using System.Drawing;
using Size = Architect.Common.Models.Size;
using Vector2 = Architect.Common.Models.Vector2;
using Architect.Widgets;
using Architect.Common.Interfaces;

namespace Architect.UI;

class TextInput : Widget, IFocusable
{

    public bool IsFocused { get => field; set => SetProperty(ref field, value); }

    public string Text
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public TextInput()
    {
        Text = string.Empty;
        BackgroundColor = Color.White;
        Position = new Vector2(0, 0);
        Size = new Size(100, 30);

        Content = new Background
        {
            Color = BackgroundColor,
            Size = Size,
            Content = new Border
            {
                OutlineRadius = 5,
                OutlineThickness = new Size(1, 1),
                Position = Position,
                Content = new Background
                {
                    Color = BackgroundColor,
                    Content = new TextBlock
                    {
                        Text = Text,
                        TextColor = Color.Black,
                        WrapText = false,
                    }
                }
            }
        };
    }

    public void SetFocus(bool focus)
    {
        throw new NotImplementedException();
    }
}
