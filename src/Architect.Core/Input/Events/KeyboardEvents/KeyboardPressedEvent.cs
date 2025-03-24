using Cosmos.System;

namespace Architect.Core.Input.Events;

/// <summary>
/// Represents an event that occurs while a key is being pressed on the keyboard.
/// </summary>
/// <param name="Key">The key that was pressed.</param>
/// <param name="Type">The type of the key event.</param>
/// <param name="KeyChar">The character associated with the key press.</param>
public record KeyboardPressedEvent(ConsoleKeyEx Key, KeyEvent.KeyEventType Type, char KeyChar) : KeyboardEvent(Key, Type, KeyChar);
