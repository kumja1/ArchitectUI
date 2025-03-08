using System.Drawing;
using Architect.Common.Interfaces;
using Architect.Core.Input;
using Architect.Core.Input.Events;
using Architect.UI.Widgets.Base;

namespace Architect.UI.Widgets.Primitives;

public class TextButton : Widget
{
    public string Text
    {
        get => GetProperty<string>(nameof(Text));
        set => SetProperty(nameof(Text), value);
    }

    public Color TextColor
    {
        get => GetProperty<Color>(nameof(TextColor));
        set => SetProperty(nameof(TextColor), value);
    }

    public EventHandler<MouseClickEvent> Clicked;

    public TextButton()
    {
        Text = "Click Me!";
        TextColor = Color.Black;
        Content = new Button
        {
            Content = new TextBlock
            {
                TextColor = TextColor,
                Text = Text,
            }
        };
    }

    public override void OnAttachToWidget(IWidget parent)
    {
        base.OnAttachToWidget(parent);
        InputManager.Instance.RegisterMouseInput(this, InputType.MouseClick, Clicked);
    }


}