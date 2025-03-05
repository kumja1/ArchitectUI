using Architect.Common.Models;

namespace Architect.Core.Input.Events;

public record class MouseHoverEvent(Vector2 Position) : InputEvent(Position);
