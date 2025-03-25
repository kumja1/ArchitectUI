using Architect.Common.Interfaces;
using Architect.Core.Input;
using Architect.Core.Input.Events;
using Architect.UI.Widgets.Base;

namespace Architect.UI.Widgets.Primitives;

public class Button : Widget
{
    public event EventHandler<InputEvent> Clicked = delegate { };

    public override void OnAttachToWidget(IWidget parent)
    {
        InputManager.Instance.RegisterMouseInput(this, InputType.MouseClick, Clicked);
        base.OnAttachToWidget(parent);
    }

    public override void OnDetachFromWidget()
    {
        InputManager.Instance.RemoveInput(this, InputType.MouseClick);
        base.OnDetachFromWidget();
    }
}
