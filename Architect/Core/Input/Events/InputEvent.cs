using Architect.Common.Models;

namespace Architect.Core.Input.Events;

public record class InputEvent(Vector2 Position, bool Handled = false);
