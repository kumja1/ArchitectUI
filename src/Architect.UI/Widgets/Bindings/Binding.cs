using System.Collections.Concurrent;
using Architect.Common.Interfaces;
using Architect.UI.Widgets.Base;

namespace Architect.UI.Widgets.Bindings;

/// <summary>
/// Represents a binding between the properties of a source and target widget, allowing for one-way or two-way data synchronization.
/// </summary>
/// <typeparam name="TSource">The type of the source widget.</typeparam>
/// <typeparam name="TTarget">The type of the target widget.</typeparam>
/// <typeparam name="TValue">The type of the value being bound.</typeparam>
public sealed class Binding<TSource, TTarget, TValue> : IDisposable
    where TSource : Widget
    where TTarget : IBindable
{
    private readonly string _sourcePropertyName;
    private readonly string _targetPropertyName;
    private readonly BindingDirection _direction;
    private readonly bool _isTwoWay;
    private readonly Action _unsubscribeSource;
    private readonly Action _unsubscribeTarget;
    private readonly ConcurrentQueue<Action> _pendingUpdates = new();
    private int _bindingDepth;

    /// <summary>
    /// Initializes a new instance of the <see cref="Binding{TSource, TTarget, TValue}"/> class.
    /// </summary>
    /// <param name="source">The source widget.</param>
    /// <param name="target">The target widget.</param>
    /// <param name="sourceGetter">A function to get the value from the source widget.</param>
    /// <param name="sourceSetter">An action to set the value on the source widget.</param>
    /// <param name="targetGetter">A function to get the value from the target widget.</param>
    /// <param name="targetSetter">An action to set the value on the target widget.</param>
    /// <param name="forwardConverter">A function to convert the value from source to target.</param>
    /// <param name="backwardConverter">A function to convert the value from target to source.</param>
    /// <param name="isTwoWay">Indicates whether the binding is two-way.</param>
    /// <param name="sourcePropertyName">The name of the source property being bound.</param>
    /// <param name="targetPropertyName">The name of the target property being bound.</param>
    internal Binding(
        TSource source,
        TTarget target,
        Func<TSource, TValue> sourceGetter,
        Action<TSource, TValue> sourceSetter,
        Func<TTarget, TValue> targetGetter,
        Action<TTarget, TValue> targetSetter,
        Func<TValue, TValue> forwardConverter,
        Func<TValue, TValue> backwardConverter,
        BindingDirection direction,
        string sourcePropertyName,
        string targetPropertyName
    )
    {
        _sourcePropertyName = sourcePropertyName;
        _targetPropertyName = targetPropertyName;
        _direction = direction;

        _isTwoWay = direction == BindingDirection.TwoWay;

        // Source -> Target binding
        if (_direction == BindingDirection.OneWayToTarget || _isTwoWay)
            _unsubscribeSource = SubscribeToPropertyChanged(
                source,
                () => UpdateBinding(source, target, sourceGetter, targetSetter, forwardConverter)
            );
        else if (_direction == BindingDirection.OneTime)
            UpdateBinding(source, target, sourceGetter, targetSetter, forwardConverter);

        // Target -> Source binding (if two-way or one-way to source)
        if (_isTwoWay || _direction == BindingDirection.OneWayToSource)
        {
            _unsubscribeTarget = SubscribeToPropertyChanged(
                target,
                () => UpdateBinding(target, source, targetGetter, sourceSetter, backwardConverter)
            );
        }
    }

    private void UpdateBinding<TSourceObj, TTargetObj>(
        TSourceObj source,
        TTargetObj target,
        Func<TSourceObj, TValue> getter,
        Action<TTargetObj, TValue> setter,
        Func<TValue, TValue> converter
    )
    {
        try
        {
            _bindingDepth++;
            setter(target, converter(getter(source)));
        }
        finally
        {
            _bindingDepth--;
            if (_bindingDepth == 0)
                ProcessPendingUpdates();
        }
    }

    public void Dispose()
    {
        _unsubscribeSource?.Invoke();
        _unsubscribeTarget?.Invoke();
    }

    private Action SubscribeToPropertyChanged<T>(T obj, Action callback)
    {
        if (obj is IBindable bindable)
        {
            void Handler(string propertyName, object _)
            {
                if (
                    (
                        propertyName == _sourcePropertyName
                        || (propertyName == _targetPropertyName && _isTwoWay)
                    )
                )
                {
                    if (_bindingDepth == 0)
                        callback();
                    else
                        _pendingUpdates.Enqueue(callback);
                }
            }

            Action<string, object> handler = Handler;
            bindable.PropertyChanged += handler;
            return () => bindable.PropertyChanged -= handler;
        }
        return () => { };
    }

    private void ProcessPendingUpdates()
    {
        if (_pendingUpdates.IsEmpty)
            return;

        while (_bindingDepth == 0 && _pendingUpdates.TryDequeue(out Action? update))
            update();
    }
}
