using Cosmos.System;

namespace Architect.Common.Interfaces.Events;



/// <summary>
/// Defines a contract for keyboard events.
/// </summary>
public interface IKeyboardEvent : IInputEvent
{
    ConsoleKeyEx Key { get; }
    KeyEvent.KeyEventType Type { get; }
    char KeyChar { get; }
}