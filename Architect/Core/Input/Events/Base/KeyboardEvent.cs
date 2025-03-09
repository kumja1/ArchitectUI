using Architect.Common.Models;
using Cosmos.System;

namespace Architect.Core.Input.Events;

/// <summary>
/// Represents a keyboard event with a specific key, event type, and character.
/// </summary>
/// <param name="Key">The key associated with the keyboard event.</param>
/// <param name="Type">The type of the keyboard event.</param>
/// <param name="KeyChar">The character associated with the key event.</param>
public record KeyboardEvent(ConsoleKeyEx Key, KeyEvent.KeyEventType Type, char KeyChar) : InputEvent(Vector2.Zero);
