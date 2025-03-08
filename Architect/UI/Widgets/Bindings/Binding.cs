
using Architect.UI.Widgets.Base;

public sealed class Binding<TSource, TTarget, TValue> : IDisposable
{
    private static int _bindingDepth;

    private readonly Action _unsubscribeSource;
    private readonly Action _unsubscribeTarget;

    private readonly string _sourcePropertyName;

    private readonly string _targetPropertyName;

    private readonly bool _isTwoWay;



    internal Binding(
        TSource source,
        TTarget target,
        Func<TSource, TValue> sourceGetter,
        Action<TSource, TValue> sourceSetter,
        Func<TTarget, TValue> targetGetter,
        Action<TTarget, TValue> targetSetter,
        Func<TValue, TValue> forwardConverter,
        Func<TValue, TValue> backwardConverter,
        bool isTwoWay,
        string sourcePropertyName,
        string targetPropertyName)
    {
        _sourcePropertyName = sourcePropertyName;
        _targetPropertyName = targetPropertyName;
        _isTwoWay = isTwoWay;

        // Source -> Target binding
        _unsubscribeSource = SubscribeToPropertyChanged(
            source,
            () => UpdateTarget(source, target, sourceGetter, targetSetter, forwardConverter));



        // Target -> Source binding (if two-way)
        if (isTwoWay)
        {
            _unsubscribeTarget = SubscribeToPropertyChanged(
                target,
                () => UpdateSource(source, target, sourceSetter, targetGetter, backwardConverter));
        }
    }

    private void UpdateTarget(
        TSource source,
        TTarget target,
        Func<TSource, TValue> sourceGetter,
        Action<TTarget, TValue> targetSetter,
        Func<TValue, TValue> converter)
    {
        try
        {
            _bindingDepth++;
            var value = converter(sourceGetter(source));
            targetSetter(target, value);
        }
        finally
        {
            _bindingDepth--;
        }
    }

    private void UpdateSource(
        TSource source,
        TTarget target,
        Action<TSource, TValue> sourceSetter,
        Func<TTarget, TValue> targetGetter,
        Func<TValue, TValue> converter)
    {
        try
        {
            _bindingDepth++;
            var value = converter(targetGetter(target));
            sourceSetter(source, value);
        }
        finally
        {
            _bindingDepth--;
        }
    }

    public void Dispose()
    {
        _unsubscribeSource?.Invoke();
        _unsubscribeTarget?.Invoke();
    }

    private Action SubscribeToPropertyChanged<T>(T obj, Action callback)
    {
        if (obj is Widget notifier)
        {
            void Handler(string propertyName, object _)
            {
                if (propertyName == _sourcePropertyName || (propertyName == _targetPropertyName && _isTwoWay))
                    callback();
            }

            Action<string, object> handler = Handler;
            notifier.PropertyChanged += handler;
            return () => notifier.PropertyChanged -= handler;
        }
        return () => { };
    }
}