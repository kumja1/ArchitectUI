using Architect.Common.Models;

namespace Architect.Core.Input.Events;

/// <summary>
/// Represents an event that occurs when the mouse enters a specific area.
/// </summary>
/// <param name="Position">The position of the mouse when the event occurs.</param>
public record class MouseEnterEvent(Vector2 Position) : InputEvent(Position);
