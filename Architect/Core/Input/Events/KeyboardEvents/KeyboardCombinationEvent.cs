using Architect.Common.Interfaces.Events;
using Cosmos.System;

namespace Architect.Core.Input.Events;

/// <summary>
/// Represents an event that occurs when a specific combination of keys is pressed on the keyboard.
/// </summary>
/// <param name="PressedKeys">A list of keys that are pressed during the event.</param>
public record KeyboardCombinationEvent(List<ConsoleKeyEx> PressedKeys) : KeyboardEvent(ConsoleKeyEx.NoName, KeyEvent.KeyEventType.Make, ' '), IKeyboardCombinationEvent;
