namespace Architect.UI.Data.Binding;

internal delegate TValue BindingGetter<in TSource, out TValue>(TSource source);

internal delegate void BindingSetter<in TSource, in TValue>(TSource source, TValue value,
    Binding binding);

public class Binding(BindingDirection direction)
{
    public readonly BindingDirection Direction = direction;
}

public sealed class Binding<TSource, TTarget, TValue> : Binding, IDisposable
    where TSource : BindableObject where TTarget : BindableObject
{
    private readonly string _sourcePropertyName;
    private readonly string _targetPropertyName;
    private readonly bool _isTwoWay;
    private readonly Action? _unsubscribeSource;
    private readonly Action? _unsubscribeTarget;

    private bool
        _isUpdating; // This is used to prevent infinite loops when updating the source or target property during a binding update.

    internal Binding(
        TSource source,
        TTarget target,
        BindingGetter<TSource, TValue> sourceGetter,
        BindingSetter<TSource, TValue> sourceSetter,
        BindingGetter<TTarget, TValue> targetGetter,
        BindingSetter<TTarget, TValue> targetSetter,
        Func<TValue, TValue> forwardConverter,
        Func<TValue, TValue> backwardConverter,
        BindingDirection direction,
        string sourcePropertyName,
        string targetPropertyName
    ) : base(direction)
    {
        _sourcePropertyName = sourcePropertyName;
        _targetPropertyName = targetPropertyName;

        _isTwoWay = direction == BindingDirection.TwoWay;

        // Source -> Target binding
        if (direction == BindingDirection.OneWayToTarget || _isTwoWay)
            _unsubscribeSource = SubscribeToPropertyChanged(
                source,
                () => UpdateBinding(source, target, sourceGetter, targetSetter, forwardConverter)
            );
        else if (direction == BindingDirection.OneTime)
            UpdateBinding(source, target, sourceGetter, targetSetter, forwardConverter);

        // Target -> Source binding (if two-way or one-way to source)
        if (_isTwoWay || direction == BindingDirection.OneWayToSource)
            _unsubscribeTarget = SubscribeToPropertyChanged(
                target,
                () => UpdateBinding(target, source, targetGetter, sourceSetter, backwardConverter)
            );
    }

    private void UpdateBinding<TSource2, TTarget2>(
        TSource2 source,
        TTarget2 target,
        BindingGetter<TSource2, TValue> getter,
        BindingSetter<TTarget2, TValue> setter,
        Func<TValue, TValue> converter
    )
    {
        try
        {
            _isUpdating = true;
            setter(target, converter(getter(source)), this);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    private Action SubscribeToPropertyChanged<T>(T bindableObject, Action callback)
        where T : BindableObject
    {
        PropertyChangedCallback handler = Handler;
        bindableObject.PropertyChanged += handler;
        return () => bindableObject.PropertyChanged -= handler;

        void Handler(string propertyName, object? _1, object _2)
        {
            if (
                propertyName != _sourcePropertyName
                || !(propertyName == _targetPropertyName && _isTwoWay)
                || _isUpdating
            )
                return;

            callback();
        }
    }

    public void Dispose()
    {
        _unsubscribeSource?.Invoke();
        _unsubscribeTarget?.Invoke();
    }
}