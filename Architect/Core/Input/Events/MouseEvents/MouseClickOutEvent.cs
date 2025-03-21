using Architect.Common.Models;
using Cosmos.System;

namespace Architect.Core.Input.Events;

/// <summary>
/// Represents an event that occurs when a mouse click is detected outside a specified area.
/// </summary>
/// <param name="Button">The state of the mouse button that was clicked.</param>
/// <param name="Position">The position of the mouse cursor at the time of the click.</param>
public record class MouseClickOutEvent(MouseState Button, Vector2 Position) : MouseEvent(Button, Position);
