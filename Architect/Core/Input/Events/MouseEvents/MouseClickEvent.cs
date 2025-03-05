
using Architect.Common.Models;
using Cosmos.System;

namespace Architect.Core.Input.Events;

public record MouseClickEvent(MouseState Button, Vector2 Position) :  InputEvent(Position);
 