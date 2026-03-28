using Cosmos.Kernel.System.Keyboard;

namespace Architect.Input.Events.Keyboard;

/// <summary>
/// Represents an event that occurs when a specific combination of keys is pressed on the keyboard.
/// </summary>
/// <param name="PressedKeys">A list of keys that are pressed during the event.</param>
public record KeyboardCombinationEvent(ConsoleKeyEx[] PressedKeys) : KeyboardEvent(KeyEvent.KeyEventType.Make);
