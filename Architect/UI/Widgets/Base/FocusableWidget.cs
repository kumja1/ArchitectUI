using Architect.Common.Interfaces;
using Architect.Core.Input;
using Architect.Core.Input.Events;
using Cosmos.System;

namespace Architect.UI.Widgets.Base;

public class FocusableWidget : Widget, IFocusableWidget
{
    public bool IsFocused
    {
        get => GetProperty<bool>(nameof(IsFocused));
        set => SetProperty(nameof(IsFocused), value);
    }
    bool IFocusableWidget.IsFocused
    {
        get => IsFocused;
        set => IsFocused = value;
    }

    /// <summary>
    /// Called when the widget gains focus.
    /// </summary>
    public virtual void OnFocus() => IsFocused = true;

    /// <summary>
    /// Called when the widget loses focus.
    /// </summary>
    public virtual void OnUnfocus() => IsFocused = false;

    public override void OnAttachToWidget(IWidget parent)
    {
        base.OnAttachToWidget(parent);

        InputManager.Instance.RegisterKeyboardInput(
            this,
            InputType.KeyboardPress,
            [ConsoleKeyEx.Escape],
            (_, _) => FocusManager.Instance.RemoveFocus(this)
        );

        InputManager.Instance.RegisterMouseInput<MouseClickEvent>(
            this,
            InputType.MouseClick,
            (sender, _) => FocusManager.Instance.SetFocus(this)
        );

        InputManager.Instance.RegisterMouseInput<MouseClickOutEvent>(
            this,
            InputType.MouseClickOut,
            (sender, _) => FocusManager.Instance.RemoveFocus(this)
        );
    }

    public override void OnDetachFromWidget()
    {
        base.OnDetachFromWidget();

        InputManager.Instance.RemoveInput(this, InputType.MouseClick);
        InputManager.Instance.RemoveInput(this, InputType.MouseClickOut);
        InputManager.Instance.RemoveInput(this, InputType.KeyboardPress);
    }
}
