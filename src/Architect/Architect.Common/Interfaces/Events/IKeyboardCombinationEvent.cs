using Cosmos.System;

namespace Architect.Common.Interfaces.Events;


/// <summary>
/// Defines a contract for keyboard combination events.
/// </summary>
public interface IKeyboardCombinationEvent : IKeyboardEvent
{
    List<ConsoleKeyEx> PressedKeys { get; }
}

