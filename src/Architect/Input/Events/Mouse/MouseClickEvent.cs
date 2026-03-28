using Architect.UI;

namespace Architect.Input.Events.Mouse;

public enum MouseButton
{
    Left,
    Right,
    Middle,
    None
}

/// <summary>
/// Represents an event that occurs when a mouse button is clicked.
/// </summary>
/// <param name="Button">The state of the mouse button that was clicked.</param>
/// <param name="Position">The position of the mouse cursor at the time of the click.</param>
public record MouseClickEvent(MouseButton Button, Vector2 Position) : MouseEvent(Position);
