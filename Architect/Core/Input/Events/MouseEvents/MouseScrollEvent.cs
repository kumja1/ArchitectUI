using Architect.Common.Models;

namespace Architect.Core.Input.Events;

public record MouseScrollEvent(Vector2 Position, int ScrollDelta) : InputEvent(Position);