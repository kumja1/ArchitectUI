using Cosmos.System;

namespace Architect.Common.Interfaces.Events;

/// <summary>
/// Defines a contract for mouse events.
/// </summary>
public interface IMouseEvent : IInputEvent
{
    MouseState Button { get; }
}
