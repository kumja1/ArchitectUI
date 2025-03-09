using Architect.Common.Models;

namespace Architect.Core.Input.Events;

/// <summary>
/// Represents an event that occurs when the mouse pointer leaves a specific area.
/// </summary>
/// <param name="Position">The position of the mouse pointer when the event occurs.</param>
public record class MouseLeaveEvent(Vector2 Position) : InputEvent(Position);
