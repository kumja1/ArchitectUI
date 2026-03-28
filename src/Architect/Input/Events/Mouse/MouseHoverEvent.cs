using Architect.UI;

namespace Architect.Input.Events.Mouse;

/// <summary>
/// Represents an event that occurs when the mouse hovers over a specific position.
/// </summary>
/// <param name="Position">The position of the mouse when the hover event occurs.</param>
public record MouseHoverEvent(Vector2 Position) : MouseEvent(Position);