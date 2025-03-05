using Cosmos.System;

namespace Architect.Core.Input.Events;

public record KeyboardReleaseEvent(ConsoleKeyEx Key, KeyEvent.KeyEventType Type, char KeyChar) : KeyboardEvent(Key, Type, KeyChar);
