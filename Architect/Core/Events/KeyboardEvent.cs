using Architect.Core.Interfaces;
using Cosmos.System;

namespace Architect.Core.Events;

record KeyboardEvent(ConsoleKeyEx Key, KeyEvent.KeyEventType Type) : IEvent;
