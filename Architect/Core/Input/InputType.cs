namespace Architect.Core.Input;

/// <summary>
/// Represents the different types of input events that can be handled.
/// </summary>
public enum InputType
{
    /// <summary>
    /// A mouse click event.
    /// </summary>
    MouseClick,

    /// <summary>
    /// A mouse click outside of a specific area.
    /// </summary>
    MouseClickOut,

    /// <summary>
    /// A keyboard event.
    /// </summary>
    Keyboard,

    /// <summary>
    /// A keyboard key press event.
    /// </summary>
    KeyboardPress,

    /// <summary>
    /// A keyboard key pressed event (key is held down).
    /// </summary>
    KeyboardPressed,

    /// <summary>
    /// A mouse scroll event.
    /// </summary>
    MouseScroll,

    /// <summary>
    /// A mouse drag event.
    /// </summary>
    MouseDrag,

    /// <summary>
    /// A mouse enter event (mouse enters a specific area).
    /// </summary>
    MouseEnter,

    /// <summary>
    /// A mouse leave event (mouse leaves a specific area).
    /// </summary>
    MouseLeave,

    /// <summary>
    /// A mouse hover event (mouse hovers over a specific area).
    /// </summary>
    MouseHover,

    /// <summary>
    /// A mouse double-click event.
    /// </summary>
    MouseDoubleClick,

    /// <summary>
    /// A keyboard key release event.
    /// </summary>
    KeyboardRelease,

    /// <summary>
    /// A keyboard combination event (multiple keys pressed simultaneously).
    /// </summary>
    KeyboardCombination,
}