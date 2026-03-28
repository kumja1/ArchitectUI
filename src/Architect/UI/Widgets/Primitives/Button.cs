using Architect.Input;
using Architect.Input.Events;

namespace Architect.UI.Widgets.Primitives;

public class Button : SingleContentWidget
{
    public event EventHandler<InputEvent> Clicked = delegate { };

    protected override void OnAttachToWidget()
    {
        InputManager.Instance.RegisterMouseInput(this, InputType.MouseClick, Clicked);
    }

    protected override void OnDetachFromWidget()
    {
        InputManager.Instance.RemoveInputHandler(this, InputType.MouseClick);
    }
}