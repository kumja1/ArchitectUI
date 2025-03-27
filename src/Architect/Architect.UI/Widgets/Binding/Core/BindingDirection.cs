namespace Architect.UI.Widgets.Binding.Core;

/// <summary>
/// Specifies the direction of data binding.
/// </summary>
public enum BindingDirection
{
    /// <summary>
    /// Data flows in one direction only, from the source to the target. This is the opposite of <see cref="OneWayToSource"/>.
    /// </summary>
    OneWayToTarget,

    /// <summary>
    /// Data flows in both directions, from the source to the target and from the target to the source.
    /// </summary>
    TwoWay,

    /// <summary>
    /// Data flows in one direction only, from the target to the source. This is the opposite of <see cref="OneWayToTarget"/>.
    /// </summary>
    OneWayToSource,

    /// <summary>
    /// Data flows in one direction only, from the source to the target. This is one time only.
    /// </summary>
    OneTime,
}
