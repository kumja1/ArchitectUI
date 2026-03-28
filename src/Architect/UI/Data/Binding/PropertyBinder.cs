using System.Runtime.CompilerServices;
using Architect.UI.Widgets;
using Architect.Utilities.Exceptions;

namespace Architect.UI.Data.Binding;

/// <summary>
/// Binds a property from a source widget to a target widget, optionally using converters and supporting two-way binding.
/// </summary>
/// <typeparam name="TSource">The type of the source widget.</typeparam>
/// <typeparam name="TValue">The type of the property value.</typeparam>
public class PropertyBinder<TSource, TValue> where TSource : Widget
{
    private readonly TSource _source;

    private readonly int _sourceFieldOffset;

    private readonly BindingGetter<TSource, TValue> _sourceGetter;

    private readonly BindingSetter<TSource, TValue> _sourceSetter;

    private readonly Func<TValue, TValue> _forwardConverter;

    private BindingDirection _direction;

    private readonly Func<TValue, TValue> _backwardConverter;
    private readonly string _sourcePropertyName;

    internal PropertyBinder(
        TSource source,
        string sourcePropertyName,
        int sourceFieldOffset,
        BindingGetter<TSource, TValue>? sourceGetter = null,
        BindingSetter<TSource, TValue>? sourceSetter = null)
    {
        _source = source;
        _sourcePropertyName = sourcePropertyName;
        _sourceFieldOffset = sourceFieldOffset;
        _forwardConverter = v => v;
        _backwardConverter = v => v;
        _sourceGetter = sourceGetter ?? (s => s.GetProperty(
            ref Unsafe.As<TSource, TValue>(ref Unsafe.AddByteOffset(ref s, _sourceFieldOffset))));
        _sourceSetter = sourceSetter ?? ((s, v, b) =>
            s.SetProperty(ref Unsafe.As<TSource, TValue>(ref Unsafe.AddByteOffset(ref s, _sourceFieldOffset)),
                v));
    }


    public PropertyBinder<TSource, TNewValue> WithConverter<TNewValue>(
        Func<TValue, TNewValue> converter,
        Func<TNewValue, TValue>? backwardConverter = null
    )
    {
        return new PropertyBinder<TSource, TNewValue>(
            _source,
            _sourcePropertyName,
            _sourceFieldOffset,
            s => converter(_sourceGetter(s)),
            backwardConverter != null
                ? (s, v, b) => _sourceSetter(s, backwardConverter(v), b)
                : null
        );
    }

    /// <summary>
    /// Sets the binding direction (one-way or two-way) for this binding.
    /// </summary>
    /// <param name="direction">The binding direction.</param>
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
    public IDisposable To<TTarget>(TTarget target, string? targetPropertyName = null) where TTarget : BindableObject
    {
        targetPropertyName ??= _sourcePropertyName;
        if (_backwardConverter == null && _direction == BindingDirection.TwoWay)
            throw new InvalidOperationException("Two-way binding requires a backward converter");

        TypeMetadata targetMetadata = BindableObject.GetTypeMetadata<TTarget>();
        if (!targetMetadata.PropertyTable.TryGetValue(targetPropertyName, out int targetFieldOffset))
            throw new FieldOffsetNotFoundException(targetPropertyName, targetMetadata.TypeName);

        Binding<TSource, TTarget, TValue> binding = new(
            source: _source,
            target: target,
            sourceGetter: _sourceGetter,
            sourceSetter: _sourceSetter,
            targetGetter: t =>
                t.GetProperty(
                    ref Unsafe.As<TTarget, TValue>(ref Unsafe.AddByteOffset(ref target, targetFieldOffset))),
            targetSetter: (t, v, b) =>
                t.SetProperty(ref Unsafe.As<TTarget, TValue>(ref Unsafe.AddByteOffset(ref target, targetFieldOffset)),
                    v, targetPropertyName),
            forwardConverter: _forwardConverter,
            backwardConverter: _backwardConverter!,
            direction: _direction,
            sourcePropertyName: _sourcePropertyName,
            targetPropertyName: targetPropertyName
        );

        _source.Bindings.Add(binding);
        return binding;
    }
}