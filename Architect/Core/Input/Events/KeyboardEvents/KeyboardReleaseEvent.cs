using Cosmos.System;

namespace Architect.Core.Input.Events;

/// <summary>
/// Represents an event that occurs when a keyboard key is released.
/// </summary>
/// <param name="Key">The key that was released.</param>
/// <param name="Type">The type of the key event.</param>
/// <param name="KeyChar">The character associated with the key.</param>
public record KeyboardReleaseEvent(ConsoleKeyEx Key, KeyEvent.KeyEventType Type, char KeyChar) : KeyboardEvent(Key, Type, KeyChar);
