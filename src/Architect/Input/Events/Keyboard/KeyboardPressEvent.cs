using Cosmos.Kernel.System.Keyboard;

namespace Architect.Input.Events.Keyboard;

/// <summary>
/// Represents an event that occurs when a key is pressed on the keyboard.
/// </summary>
/// <param name="Key">The key that was pressed.</param>
/// <param name="Type">The type of the key event.</param>
/// <param name="KeyChar">The character associated with the key press.</param>
public record KeyboardPressEvent(ConsoleKeyEx Key, ConsoleModifiers KeyModifier, char KeyChar)
    : KeyboardEvent(KeyEvent.KeyEventType.Make);