using Architect.UI.Widgets.Base;

namespace Architect.UI.Widgets.Bindings;

public class PropertyBinder<TSource, TValue> where TSource : Widget
{
    private readonly TSource _source;
    private readonly Func<TSource, TValue> _sourceGetter;
    private readonly Action<TSource, TValue> _sourceSetter;
    private readonly List<IDisposable> _bindings;
    private readonly Func<TValue, TValue> _forwardConverter;

    private bool _isTwoWay;

    private readonly string _sourcePropertyName;

    private readonly Func<TValue, TValue> _backwardConverter;

    internal PropertyBinder(
        TSource source,
        string sourcePropertyName,
        List<IDisposable> bindings,
        Func<TSource, TValue> sourceGetter = null,
       Action<TSource, TValue> sourceSetter = null)
    {
        _source = source;
        _sourceGetter = sourceGetter ?? (s => source.GetProperty<TValue>(sourcePropertyName));
        _sourceSetter = sourceSetter ?? ((s, v) => source.SetProperty(sourcePropertyName, v));
        _sourcePropertyName = sourcePropertyName;
        _bindings = bindings;
        _forwardConverter = v => v;
        _backwardConverter = v => v;
    }

    public PropertyBinder<TSource, TNewValue> WithConverter<TNewValue>(
        Func<TValue, TNewValue> converter,
        Func<TNewValue, TValue> backwardConverter = null)
    {
        if (backwardConverter == null && _isTwoWay)
            throw new InvalidOperationException(
                "Two-way binding requires a reverse converter");

        return new PropertyBinder<TSource, TNewValue>(
            _source,
            _sourcePropertyName,
            _bindings,
            s => converter(_sourceGetter(s)),
            backwardConverter != null
                ? (s, v) => _sourceSetter(s, backwardConverter(v))
                : null
        );
    }

    public PropertyBinder<TSource, TValue> WithBindingDirection(BindingDirection mode)
    {
        _isTwoWay = mode == BindingDirection.TwoWay;
        return this;
    }

    public IDisposable To<TTarget>(
        TTarget target,
        string targetPropertyName) where TTarget : Widget
    {

        var binding = new Binding<TSource, TTarget, TValue>(
            source: _source,
            target: target,
            sourceGetter: _sourceGetter,
            sourceSetter: _sourceSetter,
            targetGetter: s => s.GetProperty<TValue>(targetPropertyName),
            targetSetter: (s, v) => s.SetProperty(targetPropertyName, v),
            forwardConverter: _forwardConverter,
            backwardConverter: _backwardConverter,
            isTwoWay: _isTwoWay,
            sourcePropertyName: _sourcePropertyName,
            targetPropertyName: targetPropertyName
        );

        _bindings.Add(binding);
        return binding;
    }
}
