using Architect.Common.Models;

namespace Architect.Core.Input.Events;

/// <summary>
/// Represents an event that occurs when the mouse hovers over a specific position.
/// </summary>
/// <param name="Position">The position of the mouse when the hover event occurs.</param>
public record class MouseHoverEvent(Vector2 Position) : InputEvent(Position);
