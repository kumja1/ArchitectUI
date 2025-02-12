using Architect.Common.Models;

namespace Architect.Core.Input.Events;

record MouseScrollEvent(Vector2 Position, int ScrollDelta) : InputEvent(Position);