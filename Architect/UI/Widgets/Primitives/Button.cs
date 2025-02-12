using System.Drawing;
using Architect.Common.Interfaces;
using Architect.Core.Input;
using Architect.Core.Input.Events;

namespace Architect.UI.Primitives;

class Button : Widget
{
    public string Text { get => field; set => SetProperty(ref field, value); }
    public Color BackgroundColor { get => field; set => SetProperty(ref field, value); }

    public event EventHandler<InputEvent> Clicked;

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

    public override void OnAttachToWidget(IDrawingContext context)
    {
        base.OnAttachToWidget(context);
        InputManager.Instance.RegisterMouseInput(this, InputType.MouseClick, Clicked);
    }

    public override void OnDetachFromWidget()
    {
        base.OnDetachFromWidget();
        InputManager.Instance.Unregister(this, InputType.MouseClick);
    }
}
