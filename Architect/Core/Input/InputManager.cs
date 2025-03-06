using Architect.Common.Models;
using Architect.Common.Interfaces;
using Architect.Common.Utilities.Extensions;
using Architect.Core.Input.Events;
using Cosmos.System;

namespace Architect.Core.Input;

sealed class InputManager
{
    private static InputManager? _instance;

    private readonly Dictionary<InputType, List<(IWidget, EventHandler<InputEvent>, List<ConsoleKeyEx>?)>> _inputs = [];

    private Vector2 _lastMousePos = new((int)MouseManager.X, (int)MouseManager.Y);

    private DateTime _lastMouseClick = DateTime.MinValue;


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


    public void RegisterMouseInput(IWidget widget, InputType inputType, EventHandler<InputEvent> action) => RegisterMouseInput<InputEvent>(widget, inputType, action);

    public void RegisterKeyboardInput(IWidget widget, List<ConsoleKeyEx> keyboardKey, EventHandler<KeyboardEvent> action) => RegisterKeyboardInput<KeyboardEvent>(widget, keyboardKey, action);

    public void RegisterMouseInput<T>(IWidget widget, InputType inputType, EventHandler<T> action) where T : InputEvent
    {
        if (inputType == InputType.Keyboard) throw new InvalidDataException("Keyboard input cannot be registered as a mouse input.");
        RegisterInput(widget, inputType, (sender, e) => action(sender, (T)e), null);
    }

    public void RegisterKeyboardInput<T>(IWidget widget, List<ConsoleKeyEx> keyboardKey, EventHandler<T> action) where T : InputEvent => RegisterInput(widget, InputType.Keyboard, (sender, e) => action(sender, (T)e), keyboardKey);

    public void RegisterInput(IWidget widget, InputType inputType, EventHandler<InputEvent> action, List<ConsoleKeyEx>? key = null) => (_inputs[inputType] ??= []).Add((widget, action, key));

    public void Tick()
    {
        if (_inputs.Count == 0) return;

        foreach (var (inputType, handlers) in _inputs)
        {
            if (handlers.Count == 0) continue;
            foreach (var (widget, action, keyboardKey) in handlers)
            {
                switch (inputType)
                {
                    case InputType.MouseClickOut:
                    case InputType.MouseDoubleClick:
                    case InputType.MouseScroll:
                    case InputType.MouseClick:
                    case InputType.MouseLeave:
                    case InputType.MouseEnter:
                    case InputType.MouseHover:
                    case InputType.MouseDrag:
                        HandleMouse(widget, action, inputType);
                        break;

                    case InputType.Keyboard:
                        HandleKeyboard(widget, action, keyboardKey, inputType);
                        break;
                }

            }
        }
    }


    public void RemoveInput(IWidget widget, InputType? inputType = null)
    {
        if (!inputType.HasValue)
        {
            var input = _inputs.FirstOrDefault(x => x.Value.Any(y => y.Item1 == widget));
            if (input.Value == null) return;

            input.Value.RemoveAll(x => x.Item1 == widget);
        }
        else
        {
            _inputs[inputType.Value].RemoveAll(x => x.Item1 == widget);
        }
    }

    public void HandleMouse(IWidget widget, EventHandler<InputEvent> action, InputType inputType)
    {
        if (MouseManager.X < 0 || MouseManager.Y < 0) return;

        InputEvent? mouseEvent = GetMouseEvent(widget, inputType);
        if (mouseEvent == null)
            return;

        action.Invoke(widget, mouseEvent);

        if (mouseEvent is MouseClickEvent)
            _lastMouseClick = DateTime.Now;

        _lastMousePos = _lastMousePos with { X = (int)MouseManager.X, Y = (int)MouseManager.Y };
    }



    private void HandleKeyboard(IWidget widget, EventHandler<InputEvent> action, List<ConsoleKeyEx>? keyboardKeys, InputType inputType)
    {
        var key = KeyboardEx.ReadKey();
        if (key == null) return; // No key pressed

        InputEvent? keyEvent = GetKeyboardEvent(inputType, key, keyboardKeys);
        if (keyEvent == null) return;

        action.Invoke(widget, keyEvent);
    }


    private InputEvent? GetKeyboardEvent(InputType inputType, KeyEvent key, List<ConsoleKeyEx>? keyboardKeys)
    {
        if (keyboardKeys == null) return null;

        var isTargetKey = keyboardKeys.Contains(key.Key);
        return key switch
        {
            _ when inputType == InputType.Keyboard && isTargetKey => new KeyboardEvent(key.Key, key.Type, key.KeyChar),
            _ when inputType == InputType.KeyboardPress && key.Type == KeyEvent.KeyEventType.Make && isTargetKey => new KeyboardPressEvent(key.Key, key.Type, key.KeyChar),
            _ when inputType == InputType.KeyboardRelease && key.Type == KeyEvent.KeyEventType.Break && isTargetKey => new KeyboardReleaseEvent(key.Key, key.Type, key.KeyChar),
            _ when inputType == InputType.KeyboardCombination && keyboardKeys.All(KeyboardEx.IsKeyPressed) => new KeyboardCombinationEvent(keyboardKeys),
            _ => null
        };
    }


    private InputEvent? GetMouseEvent(IWidget widget, InputType inputType)
    {
        var mousePosition = new Vector2((int)MouseManager.X, (int)MouseManager.Y);
        var isInside = widget.HitTest(mousePosition);
        var wasInside = widget.HitTest(_lastMousePos);

        return mousePosition switch
        {
            _ when inputType == InputType.MouseScroll && MouseManager.ScrollDelta != 0 && isInside => new MouseScrollEvent(mousePosition, MouseManager.ScrollDelta),
            _ when inputType == InputType.MouseClick && MouseEx.MouseClicked && isInside => new MouseClickEvent(MouseManager.MouseState, mousePosition),
            _ when inputType == InputType.MouseDoubleClick && (DateTime.Now - _lastMouseClick).TotalMilliseconds < 500 => new MouseDoubleClickEvent(MouseManager.MouseState, mousePosition),
            _ when inputType == InputType.MouseClickOut && MouseEx.MouseClicked && !isInside => new MouseClickOutEvent(MouseManager.MouseState, mousePosition),
            _ when inputType == InputType.MouseLeave && wasInside && !isInside => new MouseLeaveEvent(mousePosition),
            _ when inputType == InputType.MouseEnter && !wasInside && isInside => new MouseEnterEvent(mousePosition),
            _ when inputType == InputType.MouseHover && isInside => new MouseHoverEvent(mousePosition),
            _ when inputType == InputType.MouseDrag && MouseEx.MouseDrag && isInside => new MouseDragEvent(MouseManager.MouseState, mousePosition),
            _ => null
        };
    }

    internal static void Initialize(ScanMapBase keyboardLayout)
    {
        _instance ??= new InputManager();
        KeyboardManager.SetKeyLayout(keyboardLayout);
    }
}
