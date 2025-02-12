using Architect.Common.Models;
using Architect.Common.Enums;
using Architect.Common.Interfaces;
using Cosmos.System;
using Console = System.Console;
using Architect.Core.Input.Events;

namespace Architect.Core.Input;

class InputManager
{
    private static InputManager? _instance;

    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("RenderManager must be initialized first");
            }
            return _instance;
        }
    }

    private readonly Dictionary<InputType, List<(IWidget, EventHandler<InputEvent>, List<ConsoleKeyEx>?)>> _inputs = [];

    public void RegisterMouseInput(IWidget widget, InputType inputType, EventHandler<InputEvent> action)
    {
        if (inputType == InputType.Keyboard) throw new InvalidDataException("Keyboard input cannot be registered as a mouse input.");
        RegisterInput(widget, inputType, action, null);
    }

    public void RegisterKeyboardInput(IWidget widget, List<ConsoleKeyEx> keyboardKey, EventHandler<InputEvent> action) => RegisterInput(widget, InputType.Keyboard, action, keyboardKey);

    public void RegisterInput(IWidget widget, InputType inputType, EventHandler<InputEvent> action, List<ConsoleKeyEx>? key = null) => (_inputs[inputType] ??= []).Add((widget, action, key));

    public void Tick()
    {
        foreach (var inputType in _inputs.Keys)
        {
            foreach (var (widget, action, keyboardKey) in _inputs[inputType])
            {
                if (inputType == InputType.MouseClick || inputType == InputType.MouseScroll)
                    HandleMouse(widget, action, inputType);
                else if (inputType == InputType.Keyboard)
                    HandleKeyboard(widget, action, keyboardKey);
            }
        }
    }

    private void HandleKeyboard(IWidget widget, EventHandler<InputEvent> action, List<ConsoleKeyEx>? keyboardKey)
    {
        if (KeyboardManager.KeyAvailable)
        {
            var key = KeyboardManager.ReadKey();
            if (keyboardKey != null && keyboardKey.Contains(key.Key))
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

    public void Unregister(IWidget widget, InputType? inputType = null) => (inputType != null ? _inputs[inputType.Value] : _inputs.FirstOrDefault(x => x.Value.Any(y => y.Item1 == widget)).Value).RemoveAll(x => x.Item1 == widget);

    public void HandleMouse(IWidget widget, EventHandler<InputEvent> action, InputType inputType)
    {
        if (MouseManager.X < 0 || MouseManager.Y < 0) return;

        InputEvent mouseEvent = GetMouseEvent();
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

    private InputEvent? GetMouseEvent()
    {
        var mousePosition = new Vector2((int)MouseManager.X, (int)MouseManager.Y);
        return mousePosition switch
        {
            _ when MouseManager.ScrollDelta != 0 => new MouseScrollEvent(mousePosition, MouseManager.ScrollDelta),
            _ when MouseManager.MouseState == MouseState.None && MouseManager.LastMouseState != MouseState.None => new MouseClickEvent(MouseManager.MouseState, mousePosition),
            _ => null
        };
    }

    internal static void Initialize(ScanMapBase keyboardLayout)
    {
        _instance ??= new InputManager();
        KeyboardManager.SetKeyLayout(keyboardLayout);
    }
}
