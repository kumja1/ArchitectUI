using Architect.UI;

namespace Architect.Input.Events;

/// <summary>
/// Represents an input event with a handled status.
/// </summary>
/// <param name="Handled">Indicates whether the input event has been handled. Default is false.</param>
public abstract record InputEvent(bool Handled = false);
