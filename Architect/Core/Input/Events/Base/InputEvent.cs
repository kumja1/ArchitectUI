using Architect.Common.Models;

namespace Architect.Core.Input.Events;

/// <summary>
/// Represents an input event with a position and a handled status.
/// </summary>
/// <param name="Position">The position of the input event.</param>
/// <param name="Handled">Indicates whether the input event has been handled. Default is false.</param>
public record class InputEvent(Vector2 Position, bool Handled = false);
