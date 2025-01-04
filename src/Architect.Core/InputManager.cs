using Architect.Common.Interfaces;
using Architect.Common.Models;
using Cosmos.System;
using Console = System.Console;
using Architect.Core.Enums;
using Architect.Core.Events;

class InputManager
{
    public delegate void InputAction(IWidget widget, BaseEvent inputEvent);

    public List<(IWidget, InputType, InputAction, ConsoleKeyEx?)> Inputs = [];

    public void RegisterMouseInput(IWidget widget, InputType inputType, InputAction action) => Inputs.Add((widget, inputType, action, null));

    public void RegisterKeyboardInput(IWidget widget, ConsoleKeyEx keyboardKey, InputAction action) => Inputs.Add((widget, InputType.Keyboard, action, keyboardKey));

    public void RegisterInput(IWidget widget, InputType inputType, InputAction action) => Inputs.Add((widget, inputType, action, null));

    public void Tick()
    {
        foreach (var group in Inputs.GroupBy<(IWidget, InputType, InputAction, ConsoleKeyEx?), object>(x => x.Item2 == InputType.MouseClick || x.Item2 == InputType.MouseScroll ? x.Item2 : x.Item4))
        {
            foreach (var (widget, inputType, action, keyboardKey) in group)

                if (inputType == InputType.MouseClick || inputType == InputType.MouseScroll)
                {
                    HandleMouse(widget, action, inputType);
                }
                else if (inputType == InputType.Keyboard)
                {
                    HandleKeyboard(widget, action, keyboardKey);
                }
        }
    }

    private void HandleKeyboard(IWidget widget, InputAction action, ConsoleKeyEx? keyboardKey)
    {
        if (KeyboardManager.KeyAvailable)
        {
            var key = KeyboardManager.ReadKey();
            if (keyboardKey == null || key.Key == keyboardKey)
            {
                action.Invoke(widget, new KeyboardEvent(key.Key, key.Type));
            }
        }
        else
        {
            Console.WriteLine("No key available. Unregistering widget");
            Unregister(widget);
        }
    }

    private void Unregister(IWidget widget) => Inputs.RemoveAll(x => x.Item1 == widget);

    public void HandleMouse(IWidget widget, InputAction action, InputType inputType)
    {
        if (MouseManager.X < 0 || MouseManager.Y < 0) return;

        var mouseEvent = GetMouseEvent();
        if (mouseEvent == null)
        {
            Console.WriteLine("No mouse event available. Unregistering widget");
            Unregister(widget);
            return;
        }

        if (inputType == InputType.MouseClick)
        {
            if (PositionHelper.Contains(widget.Position, widget.Size, (int)MouseManager.X, (int)MouseManager.Y))
            {
                action.Invoke(widget, mouseEvent);
            }
        }
        else
        {
            action.Invoke(widget, mouseEvent);
        }

    }

    public BaseEvent? GetMouseEvent()
    {
        var mousePosition = new Vector2((int)MouseManager.X, (int)MouseManager.Y);
        if (MouseManager.ScrollDelta != 0)
        {
            return new MouseScrollEvent(mousePosition, MouseManager.ScrollDelta);
        }
        else if (MouseManager.MouseState != MouseState.None)
        {
            return new MouseClickEvent(MouseManager.MouseState, mousePosition);
        }
        return null;
    }
}


