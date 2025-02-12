using Architect.Common.Models;
using Cosmos.System;

namespace Architect.Core.Input.Events;

record KeyboardEvent(ConsoleKeyEx Key, KeyEvent.KeyEventType Type) : InputEvent(Vector2.Zero);
