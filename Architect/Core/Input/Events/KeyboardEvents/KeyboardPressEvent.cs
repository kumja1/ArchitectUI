using Cosmos.System;

namespace Architect.Core.Input.Events;

public record KeyboardPressEvent(ConsoleKeyEx Key, KeyEvent.KeyEventType Type, char KeyChar) : KeyboardEvent(Key, Type, KeyChar);
