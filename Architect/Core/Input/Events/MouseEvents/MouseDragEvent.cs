
using Architect.Common.Models;
using Cosmos.System;

namespace Architect.Core.Input.Events;

/// <summary>
/// Represents an event that occurs when a mouse drag action is detected.
/// </summary>
/// <param name="Button">The state of the mouse button involved in the drag action.</param>
/// <param name="Position">The position of the mouse cursor during the drag action.</param>
public record class MouseDragEvent(MouseState Button, Vector2 Position) : InputEvent(Position);
