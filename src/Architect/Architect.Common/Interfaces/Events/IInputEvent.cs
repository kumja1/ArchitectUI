using Architect.Common.Models;

namespace Architect.Common.Interfaces.Events;

/// <summary>
/// Defines a contract for input events.
/// </summary>
public interface IInputEvent
{
    Vector2 Position { get; }
    bool Handled { get; set; }
}
