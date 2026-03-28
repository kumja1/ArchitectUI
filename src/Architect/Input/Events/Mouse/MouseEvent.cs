using Architect.UI;

namespace Architect.Input.Events.Mouse;

public record MouseEvent(Vector2 Position) : InputEvent;