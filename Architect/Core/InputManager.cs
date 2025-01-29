using Architect.Common.Interfaces;
using Architect.Common.Models;
using Architect.Core.Enums;
using Architect.Core.Events;
using Architect.Core.Interfaces;
using Cosmos.System;
using Console = System.Console;

namespace Architect.Core;

static class InputManager
{
    public delegate void InputAction(IWidget widget, IEvent inputEvent);

    public static Dictionary<InputType, List<(IWidget, EventHandler<IEvent>, ConsoleKeyEx?)>> Inputs = [];

    public static void RegisterMouseInput(IWidget widget, InputType inputType, EventHandler<IEvent> action)
    {
        if (inputType == InputType.Keyboard) throw new InvalidDataException("Keyboard input cannot be registered as a mouse input.");
        RegisterInput(widget, inputType, action, null);
    }

    public static void RegisterKeyboardInput(IWidget widget, ConsoleKeyEx keyboardKey, EventHandler<IEvent> action) => RegisterInput(widget, InputType.Keyboard, action, keyboardKey);

    public static void RegisterInput(IWidget widget, InputType inputType, EventHandler<IEvent> action, ConsoleKeyEx? key = null) => (Inputs[inputType] ??= []).Add((widget, action, key));

    public static void Tick()
    {
        foreach (var inputType in Inputs.Keys)
        {
            foreach (var (widget, action, keyboardKey) in Inputs[inputType])
            {
                if (inputType == InputType.MouseClick || inputType == InputType.MouseScroll)
                    HandleMouse(widget, action, inputType);
                else if (inputType == InputType.Keyboard)
                    HandleKeyboard(widget, action, keyboardKey);
            }
        }
    }

    private static void HandleKeyboard(IWidget widget, EventHandler<IEvent> action, ConsoleKeyEx? keyboardKey)
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

    private static void Unregister(IWidget widget) => Inputs.FirstOrDefault(x => x.Value.Any(y => y.Item1  == widget)).Value.RemoveAll(x => x.Item1 == widget);

    public static void HandleMouse(IWidget widget, EventHandler<IEvent> action, InputType inputType)
    {
        if (MouseManager.X < 0 || MouseManager.Y < 0) return;

        IMouseEvent mouseEvent = GetMouseEvent();
        if (mouseEvent == null)
        {
            Console.WriteLine("No mouse event available. Unregistering widget");
            Unregister(widget);
            return;
        }

        if (inputType == InputType.MouseClick && widget.HitTest(mouseEvent.Position))
            action.Invoke(widget, mouseEvent);
        else
            action.Invoke(widget, mouseEvent);
        

    }

    private static IMouseEvent? GetMouseEvent()
    {
        var mousePosition = new Vector2((int)MouseManager.X, (int)MouseManager.Y);
        return mousePosition switch
        {
            _ when MouseManager.ScrollDelta != 0 => new MouseScrollEvent(mousePosition, MouseManager.ScrollDelta),
            _ when MouseManager.MouseState == MouseState.None && MouseManager.LastMouseState != MouseState.None => new MouseClickEvent(MouseManager.MouseState, mousePosition),
            _ => null
        };
    }

}


