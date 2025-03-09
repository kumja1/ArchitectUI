using Cosmos.System;

namespace Architect.Core.Input.Events;

/// <summary>
/// Represents an event that occurs when a keyboard key is pressed.
/// </summary>
/// <param name="Key">The key that was pressed.</param>
/// <param name="Type">The type of the key event.</param>
/// <param name="KeyChar">The character associated with the key press.</param>
public record KeyboardPressedEvent(ConsoleKeyEx Key, KeyEvent.KeyEventType Type, char KeyChar) : KeyboardEvent(Key, Type, KeyChar);
