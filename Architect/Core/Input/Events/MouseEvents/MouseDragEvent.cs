
using Architect.Common.Models;
using Cosmos.System;

namespace Architect.Core.Input.Events;

public record class MouseDragEvent(MouseState Button, Vector2 Position) : InputEvent(Position);
