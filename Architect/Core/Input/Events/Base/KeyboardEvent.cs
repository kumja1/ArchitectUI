using Architect.Common.Models;
using Cosmos.System;

namespace Architect.Core.Input.Events;

public record KeyboardEvent(ConsoleKeyEx Key, KeyEvent.KeyEventType Type, char KeyChar) : InputEvent(Vector2.Zero);
