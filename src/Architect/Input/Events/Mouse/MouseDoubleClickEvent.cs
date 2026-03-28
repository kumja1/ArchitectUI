using Architect.UI;

namespace Architect.Input.Events.Mouse;

/// <summary>
/// Represents an event that occurs when a mouse button is double-clicked.
/// </summary>
/// <param name="Button">The state of the mouse button that was double-clicked.</param>
/// <param name="Position">The position of the mouse cursor at the time of the double-click.</param>
public record class MouseDoubleClickEvent(MouseButton Button, Vector2 Position) : MouseClickEvent(Button, Position);