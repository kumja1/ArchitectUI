using Cosmos.System;

namespace Architect.Core.Input.Events;

public record KeyboardCombinationEvent(List<ConsoleKeyEx> PressedKeys) : KeyboardEvent(ConsoleKeyEx.NoName, KeyEvent.KeyEventType.Make, ' ');
