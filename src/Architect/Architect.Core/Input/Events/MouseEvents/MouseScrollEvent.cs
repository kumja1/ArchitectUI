using Architect.Common.Interfaces.Events;
using Architect.Common.Models;
using Cosmos.System;

namespace Architect.Core.Input.Events;

/// <summary>
/// Represents an event that occurs when the mouse scroll wheel is used.
/// </summary>
/// <param name="Position">The position of the mouse cursor when the scroll event occurred.</param>
/// <param name="ScrollDelta">The amount of scrolling that occurred. Positive values indicate scrolling up, and negative values indicate scrolling down.</param>
public record MouseScrollEvent(Vector2 Position, int ScrollDelta) : MouseEvent(MouseState.None, Position), IMouseScrollEvent;
