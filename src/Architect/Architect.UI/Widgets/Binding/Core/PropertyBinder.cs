using Architect.UI.Widgets.Base;
using Architect.UI.Widgets.Binding.Interfaces;

namespace Architect.UI.Widgets.Binding.Core;

/// <summary>
/// Binds a property from a source widget to a target widget, optionally using converters and supporting two-way binding.
/// </summary>
/// <typeparam name="TSource">The type of the source widget.</typeparam>
/// <typeparam name="TValue">The type of the property value.</typeparam>
public class PropertyBinder<TSource, TValue>
    where TSource : Widget
{
    private readonly TSource _source;

    private readonly Func<TSource, TValue> _sourceGetter;

    private readonly Action<TSource, TValue, IBinding?> _sourceSetter;

    private readonly List<IDisposable> _sourceBindings;

    private readonly Func<TValue, TValue> _forwardConverter;

    private BindingDirection _direction;

    private readonly string _sourcePropertyName;

    private readonly Func<TValue, TValue> _backwardConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBinder{TSource, TValue}"/> class.
    /// </summary>
    /// <param name="source">The source widget.</param>
    /// <param name="sourcePropertyName">The name of the property to bind on the source widget.</param>
    /// <param name="sourceBindings"></param>
    /// <param name="bindings">A shared list of bindings to which this binding will be added.</param>
    /// <param name="sourceGetter">Optional getter for the source property. If not provided, the widget's GetProperty method is used.</param>
    /// <param name="sourceSetter">Optional setter for the source property. If not provided, the widget's SetProperty method is used.</param>
    internal PropertyBinder(
        TSource source,
        string sourcePropertyName,
        List<IDisposable> sourceBindings,
        Func<TSource, TValue>? sourceGetter = null,
        Action<TSource, TValue, IBinding>? sourceSetter = null
    )
    {
        _source = source;
        _sourceGetter = sourceGetter ?? (s => source.GetProperty<TValue>(sourcePropertyName));
        _sourceSetter = sourceSetter ?? ((s, v, b) => source.SetProperty(sourcePropertyName, v, b));
        _sourcePropertyName = sourcePropertyName;
        _sourceBindings = sourceBindings;
        _forwardConverter = v => v;
        _backwardConverter = v => v;
    }

    /// <summary>
    /// Applies a converter to the binding so that the source value is transformed before assignment.
    /// </summary>
    /// <typeparam name="TNewValue">The type of the new value after conversion.</typeparam>
    /// <param name="converter">Function to convert from TValue to TNewValue.</param>
    /// <param name="backwardConverter">
    /// Optional reverse conversion from TNewValue back to TValue. Required for two-way binding.
    /// </param>
    /// <returns>A new <see cref="PropertyBinder{TSource, TNewValue}"/> configured with the converter.</returns>
    public PropertyBinder<TSource, TNewValue> WithConverter<TNewValue>(
        Func<TValue, TNewValue> converter,
        Func<TNewValue, TValue>? backwardConverter = null
    )
    {
        return new PropertyBinder<TSource, TNewValue>(
            _source,
            _sourcePropertyName,
            _sourceBindings,
            s => converter(_sourceGetter(s)),
            backwardConverter != null
                ? (s, v, b) => _sourceSetter(s, backwardConverter(v), b)
                : null
        );
    }

    /// <summary>
    /// Sets the binding direction (one-way or two-way) for this binding.
    /// </summary>
    /// <param name="mode">The binding direction mode.</param>
    /// <returns>This instance for method chaining.</returns>
    public PropertyBinder<TSource, TValue> WithBindingDirection(BindingDirection direction)
    {
        _direction = direction;
        return this;
    }

    /// <summary>
    /// Completes the binding by linking the source property to the target property.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target widget.</typeparam>
    /// <param name="target">The target widget.</param>
    /// <param name="targetPropertyName">The property name on the target widget to bind to. If not provided, sourcePropertyName is used instead </param>
    /// <returns>An <see cref="IDisposable"/> representing the binding subscription.</returns>
    public IDisposable To<TTarget>(TTarget target, string targetPropertyName = null)
        where TTarget : IBindable
    {
        if (_backwardConverter == null && _direction == BindingDirection.TwoWay)
            throw new InvalidOperationException("Two-way binding requires a reverse converter");

        targetPropertyName ??= _sourcePropertyName;

        var binding = new Binding<TSource, TTarget, TValue>(
            source: _source,
            target: target,
            sourceGetter: _sourceGetter,
            sourceSetter: _sourceSetter,
            targetGetter: s => s.GetProperty<TValue>(targetPropertyName),
            targetSetter: (s, v, b) => s.SetProperty(targetPropertyName, v, b),
            forwardConverter: _forwardConverter,
            backwardConverter: _backwardConverter!,
            direction: _direction,
            sourcePropertyName: _sourcePropertyName,
            targetPropertyName: targetPropertyName
        );

        _sourceBindings.Add(binding);
        return binding;
    }
}
