using Architect.Common.Models;
using Architect.Common.Interfaces;
using Architect.Common.Utilities.Extensions;
using Architect.Core.Input.Events;
using Cosmos.System;

namespace Architect.Core.Input;

/// <summary>
/// Manages input events for widgets, including mouse and keyboard inputs.
/// </summary>
sealed class InputManager
{
    /// <summary>
    /// Singleton instance of InputManager.
    /// </summary>
    private static InputManager? _instance;

    /// <summary>
    /// Dictionary to store input handlers for different input types.
    /// </summary>
    private readonly Dictionary<InputType, List<(IWidget, EventHandler<InputEvent>, List<ConsoleKeyEx>?)>> _inputs = [];

    /// <summary>
    /// Last recorded mouse position.
    /// </summary>
    private Vector2 _lastMousePos = new((int)MouseManager.X, (int)MouseManager.Y);

    /// <summary>
    /// Last recorded mouse click time.
    /// </summary>
    private DateTime _lastMouseClick = DateTime.MinValue;

    /// <summary>
    /// Gets the singleton instance of InputManager.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if RenderManager is not initialized first.</exception>
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

    /// <summary>
    /// Registers a mouse input handler with the default InputEvent type.
    /// </summary>
    /// <param name="widget">The widget to register the input for.</param>
    /// <param name="inputType">The type of input to register.</param>
    /// <param name="action">The event handler to invoke when the input is detected.</param>
    public void RegisterMouseInput(IWidget widget, InputType inputType, EventHandler<InputEvent> action) => RegisterMouseInput<InputEvent>(widget, inputType, action);

    /// <summary>
    /// Registers a mouse input handler with a specified InputEvent type.
    /// </summary>
    /// <typeparam name="T">The type of InputEvent.</typeparam>
    /// <param name="widget">The widget to register the input for.</param>
    /// <param name="inputType">The type of input to register.</param>
    /// <param name="action">The event handler to invoke when the input is detected.</param>
    /// <exception cref="InvalidDataException">Thrown if the input type is a keyboard input type.</exception>
    public void RegisterMouseInput<T>(IWidget widget, InputType inputType, EventHandler<T> action) where T : InputEvent
    {
        if (inputType is InputType.Keyboard or InputType.KeyboardPress or InputType.KeyboardRelease or InputType.KeyboardCombination) 
            throw new InvalidDataException("Mouse input cannot be registered as a keyboard input.");
        RegisterInput(widget, inputType, (sender, e) => action(sender, (T)e), null);
    }

    /// <summary>
    /// Registers a keyboard input handler with the default KeyboardEvent type.
    /// </summary>
    /// <typeparam name="TTarget">The type of the widget.</typeparam>
    /// <param name="widget">The widget to register the input for.</param>
    /// <param name="inputType">The type of input to register.</param>
    /// <param name="keyboardKey">The list of keyboard keys to register.</param>
    /// <param name="action">The event handler to invoke when the input is detected.</param>
    public void RegisterKeyboardInput<TTarget>(TTarget widget, InputType inputType, List<ConsoleKeyEx> keyboardKey, EventHandler<KeyboardEvent> action) where TTarget : IFocusableWidget => RegisterKeyboardInput<TTarget, KeyboardEvent>(widget, inputType, keyboardKey, action);

    /// <summary>
    /// Registers a keyboard input handler with a specified KeyboardEvent type.
    /// </summary>
    /// <typeparam name="TTarget">The type of the widget.</typeparam>
    /// <typeparam name="TEvent">The type of KeyboardEvent.</typeparam>
    /// <param name="widget">The widget to register the input for.</param>
    /// <param name="inputType">The type of input to register.</param>
    /// <param name="keyboardKey">The list of keyboard keys to register.</param>
    /// <param name="action">The event handler to invoke when the input is detected.</param>
    /// <exception cref="InvalidDataException">Thrown if the input type is a mouse input type.</exception>
    public void RegisterKeyboardInput<TTarget, TEvent>(TTarget widget, InputType inputType, List<ConsoleKeyEx> keyboardKey, EventHandler<TEvent> action)
        where TTarget : IFocusableWidget
        where TEvent : KeyboardEvent
    {
        if (inputType is not (InputType.Keyboard or InputType.KeyboardPress or InputType.KeyboardRelease or InputType.KeyboardCombination))
            throw new InvalidDataException("Keyboard input cannot be registered as a mouse input.");

        RegisterInput(widget, inputType, (sender, e) => action(sender, (TEvent)e), keyboardKey);
    }

    /// <summary>
    /// Registers an input handler for a widget.
    /// </summary>
    /// <param name="widget">The widget to register the input for.</param>
    /// <param name="inputType">The type of input to register.</param>
    /// <param name="action">The event handler to invoke when the input is detected.</param>
    /// <param name="key">The list of keyboard keys to register (optional).</param>
    public void RegisterInput(IWidget widget, InputType inputType, EventHandler<InputEvent> action, List<ConsoleKeyEx>? key = null) => (_inputs[inputType] ??= []).Add((widget, action, key));

    /// <summary>
    /// Processes input events.
    /// </summary>
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
                    case InputType.MouseClick or InputType.MouseDoubleClick or InputType.MouseClickOut or InputType.MouseEnter or InputType.MouseLeave or InputType.MouseHover or InputType.MouseScroll or InputType.MouseDrag:
                        HandleMouse(widget, action, inputType);
                        break;

                    case InputType.Keyboard or InputType.KeyboardPress or InputType.KeyboardRelease or InputType.KeyboardCombination:
                        if (widget is IFocusableWidget focusableWidget && focusableWidget.IsFocused)
                            HandleKeyboard(widget, action, keyboardKey, inputType);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Removes an input handler for a widget.
    /// </summary>
    /// <param name="widget">The widget to remove the input handler for.</param>
    /// <param name="inputType">The type of input to remove (optional).</param>
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

    /// <summary>
    /// Handles mouse input events.
    /// </summary>
    /// <param name="widget">The widget to handle the input for.</param>
    /// <param name="action">The event handler to invoke when the input is detected.</param>
    /// <param name="inputType">The type of input to handle.</param>
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

    /// <summary>
    /// Handles keyboard input events.
    /// </summary>
    /// <param name="widget">The widget to handle the input for.</param>
    /// <param name="action">The event handler to invoke when the input is detected.</param>
    /// <param name="keyboardKeys">The list of keyboard keys to handle.</param>
    /// <param name="inputType">The type of input to handle.</param>
    private void HandleKeyboard(IWidget widget, EventHandler<InputEvent> action, List<ConsoleKeyEx>? keyboardKeys, InputType inputType)
    {
        var key = KeyboardEx.ReadKey();
        if (key == null) return; // No key pressed

        InputEvent? keyEvent = GetKeyboardEvent(inputType, key, keyboardKeys);
        if (keyEvent == null) return;

        action.Invoke(widget, keyEvent);
    }

    /// <summary>
    /// Gets a keyboard event based on the input type and key.
    /// </summary>
    /// <param name="inputType">The type of input.</param>
    /// <param name="key">The key event.</param>
    /// <param name="keyboardKeys">The list of keyboard keys.</param>
    /// <returns>The corresponding keyboard event, or null if no event matches.</returns>
    private InputEvent? GetKeyboardEvent(InputType inputType, KeyEvent key, List<ConsoleKeyEx>? keyboardKeys)
    {
        if (keyboardKeys == null) return null;

        var isTargetKey = keyboardKeys.Contains(key.Key);
        return key switch
        {
            _ when inputType == InputType.Keyboard && isTargetKey => new KeyboardEvent(key.Key, key.Type, key.KeyChar),
            _ when inputType == InputType.KeyboardPress && isTargetKey && KeyboardEx.CheckKeyPress() => new KeyboardPressEvent(key.Key, key.Type, key.KeyChar),
            _ when inputType == InputType.KeyboardPressed && isTargetKey && key.Type == KeyEvent.KeyEventType.Make => new KeyboardPressedEvent(key.Key, key.Type, key.KeyChar),
            _ when inputType == InputType.KeyboardRelease && isTargetKey && key.Type == KeyEvent.KeyEventType.Break  => new KeyboardReleaseEvent(key.Key, key.Type, key.KeyChar),
            _ when inputType == InputType.KeyboardCombination && keyboardKeys.All(KeyboardEx.IsKeyBeingPressed) => new KeyboardCombinationEvent(keyboardKeys),
            _ => null
        };
    }

    /// <summary>
    /// Gets a mouse event based on the input type and mouse position.
    /// </summary>
    /// <param name="widget">The widget to get the event for.</param>
    /// <param name="inputType">The type of input.</param>
    /// <returns>The corresponding mouse event, or null if no event matches.</returns>
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

    /// <summary>
    /// Initializes the InputManager with a keyboard layout.
    /// </summary>
    /// <param name="keyboardLayout">The keyboard layout to use.</param>
    internal static void Initialize(ScanMapBase keyboardLayout)
    {
        _instance ??= new InputManager();
        KeyboardManager.SetKeyLayout(keyboardLayout);
    }
}
