using System.Drawing;
using Architect.Common.Interfaces;
using Architect.Core.Input;
using Architect.Core.Input.Events;
using Architect.UI.Widgets.Base;

namespace Architect.UI.Widgets.Primitives;

class Button : Widget
{
    public event EventHandler<InputEvent> Clicked;


    public override void OnAttachToWidget(IWidget parent)
    {
        base.OnAttachToWidget(parent);
        if (Clicked != null)
            InputManager.Instance.RegisterMouseInput(this, InputType.MouseClick, Clicked);
    }

    public override void OnDetachFromWidget()
    {
        base.OnDetachFromWidget();
        if (Clicked != null)
            InputManager.Instance.RemoveInput(this, InputType.MouseClick);
    }
}
