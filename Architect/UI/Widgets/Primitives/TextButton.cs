using System.Drawing;
using Architect.Common.Interfaces;
using Architect.Core.Input;
using Architect.Core.Input.Events;
using Architect.UI.Base;
using Architect.UI.Primitives;

public class TextButton : Widget
{
    public string Text { get => field; set => SetProperty(ref field, value); }

    public Color TextColor { get => field; set => SetProperty(ref field, value); }

    public EventHandler<MouseClickEvent> Clicked;

    public TextButton()
    {
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