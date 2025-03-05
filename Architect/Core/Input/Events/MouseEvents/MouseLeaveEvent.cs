using Architect.Common.Models;

namespace Architect.Core.Input.Events;

public record class MouseLeaveEvent(Vector2 Position) : InputEvent(Position);
