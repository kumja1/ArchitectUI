using System.Runtime.CompilerServices;
using Architect.Input.Events;
using Architect.Input.Events.Keyboard;
using Architect.Input.Events.Mouse;
using Architect.UI;
using Architect.UI.Widgets;
using Architect.Utilities.Extensions;
using Cosmos.Kernel.System.Keyboard;
using Cosmos.Kernel.System.Mouse;

namespace Architect.Input;

/// <summary>
/// Manages input events for widgets, including mouse and keyboard inputs.
/// </summary>
public sealed class InputManager
{
    /// <summary>
    /// Singleton instance of InputManager.
    /// </summary>
    private static InputManager? _instance;

    /// <summary>
    /// Gets the singleton instance of InputManager.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if InputManager is not initialized first.</exception>
    public static InputManager Instance =>
        _instance ?? throw new InvalidOperationException("RenderManager must be initialized first");

    /// <summary>
    /// Dictionary to store input handlers for different input types.
    /// </summary>
    private readonly Dictionary<InputType, List<InputHandler>> _inputs = [];


    /// <summary>
    /// Last recorded mouse position.
    /// </summary>
    private Vector2 _lastMousePos = new(MouseManager.X, MouseManager.Y);

    /// <summary>
    /// Last recorded mouse click time.
    /// </summary>
    private DateTime _lastMouseClick = DateTime.MinValue;

    /// <summary>
    /// Registers a mouse input handler with the default InputEvent type.
    /// </summary>
    /// <param name="widget">The widget to register the input for.</param>
    /// <param name="inputType">The type of input to register.</param>
    /// <param name="action">The event handler to invoke when the input is detected.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RegisterMouseInput(
        Widget widget,
        InputType inputType,
        EventHandler<MouseEvent> action
    ) => RegisterMouseInput<MouseEvent>(widget, inputType, action);

    /// <summary>
    /// Registers a mouse input handler with a specified InputEvent type.
    /// </summary>
    /// <typeparam name="TEvent">The type of MouseEvent.</typeparam>
    /// <param name="widget">The widget to register the input for.</param>
    /// <param name="inputType">The type of input to register.</param>
    /// <param name="action">The event handler to invoke when the input is detected.</param>
    /// <exception cref="InvalidDataException">Thrown if the input type is a keyboard input type.</exception>
    public void RegisterMouseInput<TEvent>(Widget widget, InputType inputType, EventHandler<TEvent> action)
        where TEvent : MouseEvent
    {
        if (
            inputType
            is InputType.Keyboard
            or InputType.KeyboardPress
            or InputType.KeyboardRelease
            or InputType.KeyboardCombination
        )
            throw new ArgumentOutOfRangeException(nameof(inputType),
                "Keyboard input cannot be registered as a mouse input.");
        RegisterInput(widget, inputType, (sender, e) => action(sender, (TEvent)e), null);
    }

    /// <summary>
    /// Registers a keyboard input handler with a specified KeyboardEvent type.
    /// </summary>
    /// <typeparam name="TEvent">The type of KeyboardEvent.</typeparam>
    /// <param name="widget">The widget to register the input for.</param>
    /// <param name="inputType">The type of input to register.</param>
    /// <param name="keys">The list of keyboard keys to register.</param>
    /// <param name="action">The event handler to invoke when the input is detected.</param>
    /// <exception cref="InvalidDataException">Thrown if the input type is a mouse input type.</exception>
    public void RegisterKeyboardInput<TEvent>(
        Widget widget,
        InputType inputType,
        ConsoleKeyEx[] keys,
        EventHandler<TEvent> action
    )
        where TEvent : KeyboardEvent
    {
        if (
            inputType
            is not (
            InputType.Keyboard
            or InputType.KeyboardPress
            or InputType.KeyboardRelease
            or InputType.KeyboardCombination
            )
        )
            throw new InvalidDataException("Keyboard input cannot be registered as a mouse input.");

        RegisterInput(widget, inputType, (sender, e) => action(sender, (TEvent)e), keys);
    }

    /// <summary>
    /// Registers an input handler for a widget.
    /// </summary>
    /// <param name="widget">The widget to register the input for.</param>
    /// <param name="inputType">The type of input to register.</param>
    /// <param name="action">The event handler to invoke when the input is detected.</param>
    /// <param name="keys">The list of keyboard keys to register (optional).</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RegisterInput(
        Widget widget,
        InputType inputType,
        EventHandler<InputEvent> action,
        ConsoleKeyEx[]? keys
    ) => _inputs[inputType].Add(new InputHandler(widget, action, keys));

    /// <summary>
    /// Processes input events.
    /// </summary>
    public void Tick()
    {
        if (_inputs.Count == 0)
            return;


        foreach ((InputType inputType, List<InputHandler> handlers) in _inputs)
        {
            if (handlers.Count == 0)
                continue;
        }
    }

    /// <summary>
    /// Removes an input handler for a widget.
    /// </summary>
    /// <param name="widget">The widget to remove the input handler for.</param>
    /// <param name="inputType">The type of input to remove (optional).</param>
    public void RemoveInputHandler(Widget widget, InputType? inputType = null)
    {
        if (!inputType.HasValue)
        {
            KeyValuePair<InputType, List<InputHandler>> input =
                _inputs.FirstOrDefault(x => x.Value.Any(y => y.Widget == widget));
            if (input.Value == null)
                return;

            input.Value.RemoveAll(x => x.Widget == widget);
        }
        else
            _inputs[inputType.Value].RemoveAll(x => x.Widget == widget);
    }

    /// <summary>
    /// Handles mouse input events.
    /// </summary>
    /// <param name="widget">The singleContentWidget to handle the input for.</param>
    /// <param name="action">The event handler to invoke when the input is detected.</param>
    /// <param name="inputType">The type of input to handle.</param>
    private void HandleMouse(Widget widget, EventHandler<InputEvent> action,
        InputType inputType)
    {
        if (MouseManager.X < 0 || MouseManager.Y < 0)
            return;

        InputEvent? mouseEvent = GetMouseEvent(widget, inputType);
        if (mouseEvent == null)
            return;

        action.Invoke(widget, mouseEvent);

        if (mouseEvent is MouseClickEvent)
            _lastMouseClick = DateTime.Now;

        _lastMousePos = new Vector2(X: MouseManager.X, Y: MouseManager.Y);
    }

    /// <summary>
    /// Handles keyboard input events.
    /// </summary>
    /// <param name="widget">The widget to handle the input for.</param>
    /// <param name="action">The event handler to invoke when the input is detected.</param>
    /// <param name="keyboardKeys">The list of keyboard keys to handle.</param>
    /// <param name="inputType">The type of input to handle.</param>
    private void HandleKeyboard(
        Widget widget,
        EventHandler<InputEvent> action,
        ConsoleKeyEx[]? keys,
        InputType inputType
    )
    {
        KeyEvent? key = KeyboardEx.ReadKey();
        if (key == null)
            return; // No key pressed

        InputEvent? keyEvent = GetKeyboardEvent(inputType, key, keys);
        if (keyEvent == null)
            return;

        action.Invoke(widget, keyEvent);
    }

    /// <summary>
    /// Gets a keyboard event based on the input type and key.
    /// </summary>
    /// <param name="inputType">The type of input.</param>
    /// <param name="key">The key event.</param>
    /// <param name="keys">The list of keyboard keys.</param>
    /// <returns>The corresponding keyboard event, or null if no event matches.</returns>
    private InputEvent? GetKeyboardEvent(
        InputType inputType,
        KeyEvent key,
        ConsoleKeyEx[]? keys
    )
    {
        if (keys == null)
            return null;

        bool isTargetKey = keys.Contains(key.Key);
        return key switch
        {
            _
                when inputType == InputType.KeyboardPress
                     && isTargetKey
                     && KeyboardEx.CheckKeyPress() => new KeyboardPressEvent(
                    key.Key,
                    key.Modifiers,
                    key.KeyChar
                ),
            _
                when inputType == InputType.KeyboardRelease
                     && isTargetKey
                     && key.Type == KeyEvent.KeyEventType.Break => new KeyboardReleaseEvent(
                    key.Key,
                    key.Modifiers,
                    key.KeyChar
                ),
            _
                when inputType == InputType.KeyboardCombination
                     && keys.All(KeyboardEx.IsKeyBeingPressed) =>
                new KeyboardCombinationEvent(keys),
            _ => null,
        };
    }

    /// <summary>
    /// Gets a mouse event based on the input type and mouse position.
    /// </summary>
    /// <param name="widget">The singleContentWidget to get the event for.</param>
    /// <param name="inputType">The type of input.</param>
    /// <returns>The corresponding mouse event, or null if no event matches.</returns>
    private InputEvent? GetMouseEvent(Widget widget, InputType inputType)
    {
        Vector2 mousePosition = new(MouseManager.X, MouseManager.Y);
        MouseButton mouseButton = MouseEx.GetButton();
        bool clicked = mouseButton != MouseButton.None;
        bool isInside =
            Vector2.Within(mousePosition, widget.X, widget); // TODO: Consider replacing with Rect.Contains (add method)
        return mousePosition switch
        {
            _
                when inputType == InputType.MouseScroll
                     && MouseManager.ScrollDelta != 0
                     && isInside => new MouseScrollEvent(mousePosition, MouseManager.ScrollDelta),
            _ when inputType == InputType.MouseClick && clicked && isInside =>
                new MouseClickEvent(mouseButton, mousePosition),
            _
                when inputType == InputType.MouseDoubleClick
                     && (DateTime.Now - _lastMouseClick).TotalMilliseconds < 500 =>
                new MouseDoubleClickEvent(mouseButton, mousePosition),
            _ when inputType == InputType.MouseClickOut && clicked && !isInside =>
                new MouseClickOutEvent(mouseButton, mousePosition),
            _ when inputType == InputType.MouseLeave && wasInside && !isInside =>
                new MouseLeaveEvent(mousePosition),
            _ when inputType == InputType.MouseEnter && !wasInside && isInside =>
                new MouseEnterEvent(mousePosition),
            _ when inputType == InputType.MouseHover && isInside => new MouseHoverEvent(
                mousePosition
            ),
            _ when inputType == InputType.MouseDrag && MouseEx.MouseDrag && isInside =>
                new MouseDragEvent(MouseManager..MouseState, mousePosition),
            _ => null,
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