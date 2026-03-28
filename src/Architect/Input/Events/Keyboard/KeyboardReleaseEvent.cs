using Cosmos.Kernel.System.Keyboard;

namespace Architect.Input.Events.Keyboard;

/// <summary>
/// Represents an event that occurs when a keyboard key is released.
/// </summary>
/// <param name="Key">The key that was released.</param>
/// <param name="KeyChar">The character associated with the key.</param>
public record KeyboardReleaseEvent(ConsoleKeyEx Key, ConsoleModifiers KeyModifier, char KeyChar) : KeyboardEvent(KeyEvent.KeyEventType.Break);