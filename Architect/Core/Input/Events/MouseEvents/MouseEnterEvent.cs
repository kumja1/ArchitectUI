using Architect.Common.Models;

namespace Architect.Core.Input.Events;

public record class MouseEnterEvent(Vector2 Position) : InputEvent(Position);
