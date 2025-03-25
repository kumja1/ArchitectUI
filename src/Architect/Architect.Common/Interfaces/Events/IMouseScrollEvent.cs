namespace Architect.Common.Interfaces.Events;



/// <summary>
/// Defines a contract for mouse scroll events.
/// </summary>
public interface IMouseScrollEvent : IInputEvent
{
    int ScrollDelta { get; }
}