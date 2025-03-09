namespace Architect.UI.Widgets.Bindings;


/// <summary>
/// Specifies the direction of data binding.
/// </summary>
public enum BindingDirection
{
    /// <summary>
    /// Data flows in one direction only, from the source to the target.
    /// </summary>
    OneWay,

    /// <summary>
    /// Data flows in both directions, from the source to the target and from the target to the source.
    /// </summary>
    TwoWay
}

