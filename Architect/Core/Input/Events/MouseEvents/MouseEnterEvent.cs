using Architect.Common.Models;
using Cosmos.System;

namespace Architect.Core.Input.Events;

/// <summary>
/// Represents an event that occurs when the mouse enters a specific area.
/// </summary>
/// <param name="Position">The position of the mouse when the event occurs.</param>
public record class MouseEnterEvent(Vector2 Position) : MouseEvent(MouseState.None, Position);
