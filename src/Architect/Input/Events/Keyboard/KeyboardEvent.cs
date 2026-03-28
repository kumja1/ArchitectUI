using Cosmos.Kernel.System.Keyboard;

namespace Architect.Input.Events.Keyboard;

/// <summary>
/// Represents a keyboard event with a specific key, event type, and character.
/// </summary>
/// <param name="Type">The type of the keyboard event.</param>
public record KeyboardEvent(KeyEvent.KeyEventType Type) : InputEvent;