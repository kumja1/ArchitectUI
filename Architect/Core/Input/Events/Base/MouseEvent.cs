using Cosmos.System;
using Architect.Common.Models;
using Architect.Common.Interfaces.Events;

namespace Architect.Core.Input.Events;


public record MouseEvent(MouseState Button, Vector2 Position) : InputEvent(Position), IMouseEvent;
