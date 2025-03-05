using System.Drawing;
using Architect.Common.Interfaces;
using Architect.Core.Input;
using Architect.Core.Input.Events;
using Architect.UI.Base;

namespace Architect.UI.Primitives;

class Button : Widget
{
    public Color BackgroundColor { get => field; set => SetProperty(ref field, value); } = Color.White;

    public event EventHandler<InputEvent> Clicked;


    public override void OnAttachToWidget(IWidget parent)
    {
        base.OnAttachToWidget(parent);
        InputManager.Instance.RegisterMouseInput(this, InputType.MouseClick, Clicked);
    }

    public override void OnDetachFromWidget()
    {
        base.OnDetachFromWidget();
        InputManager.Instance.RemoveInput(this, InputType.MouseClick);
    }
}
