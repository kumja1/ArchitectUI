using Cosmos.System;

namespace Architect.Core.Events;

record KeyboardEvent(ConsoleKeyEx Key, KeyEvent.KeyEventType Type) : BaseEvent;
