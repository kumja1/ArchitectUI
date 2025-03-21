
using Architect.Common.Models;
using Cosmos.System;

namespace Architect.Core.Input.Events;

/// <summary>
/// Represents an event that occurs when a mouse button is double-clicked.
/// </summary>
/// <param name="Button">The state of the mouse button that was double-clicked.</param>
/// <param name="Position">The position of the mouse cursor at the time of the double-click.</param>
public record class MouseDoubleClickEvent(MouseState Button, Vector2 Position) : MouseEvent(Button, Position);
