using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.CompilerServices;
using Architect.Rendering;
using Architect.UI.Data.Binding;
using Architect.UI.Widgets.Layout.Alignment;
using Architect.Utilities.Exceptions;
using Cosmos.Kernel.System.Graphics;

namespace Architect.UI.Widgets;

public enum PropertyFlag
{
    None,
    AffectsMeasure,
    AffectsArrange,
    AffectsParentMeasure,
    AffectsParentArrange,
    AffectsVisual,
    NotDataBindable,
}

public abstract class Widget : BindableObject, IDisposable
{
    internal Size DesiredSize = Size.Zero;

    internal readonly List<IDisposable> Bindings = [];

    private bool _isDirty;
    private bool _isDisposed;
    private Widget? _parent;

    public override event PropertyChangedCallback PropertyChanged = delegate { };

    public bool IsVisible
    {
        get => GetProperty(ref field, initialValue: true);
        set => SetProperty(ref field, value, flag: PropertyFlag.AffectsVisual);
    }

    public int ZIndex
    {
        get => GetProperty(ref field, initialValue: 0);
        set => SetProperty(ref field, value, flag: PropertyFlag.AffectsVisual);
    }

    public int X
    {
        get => GetProperty(ref field, initialValue: 0);
        set => SetProperty(ref field, value);
    }

    public int Y
    {
        get => GetProperty(ref field, initialValue: 0);
        set => SetProperty(ref field, value);
    }

    public int Width
    {
        get => GetProperty(ref field, initialValue: 0);
        set => SetProperty(ref field, value);
    }

    public int Height
    {
        get => GetProperty(ref field, initialValue: 0);
        set => SetProperty(ref field, value);
    }

    public HorizontalAlignment HorizontalAlignment
    {
        get => GetProperty(ref field);
        set => SetProperty(ref field, value);
    }
    
    public VerticalAlignment VerticalAlignment
    {
        get => GetProperty(ref field);
        set => SetProperty(ref field, value);
    }

    public Color BackgroundColor
    {
        get => GetProperty(ref field, initialValue: Color.White);
        set => SetProperty(ref field, value);
    }

    public EdgeInsets Padding
    {
        get => GetProperty(ref field, initialValue: EdgeInsets.Zero);
        set => SetProperty(ref field, value);
    }

    public EdgeInsets Margin
    {
        get => GetProperty(ref field, initialValue: EdgeInsets.Zero);
        set => SetProperty(ref field, value);
    }

    public int GridColumn
    {
        get => GetProperty(ref field);
        set => SetProperty(ref field, value);
    }

    public int GridRow
    {
        get => GetProperty(ref field);
        set => SetProperty(ref field, value);
    }

    internal void OnAttachToWidgetInternal(Widget parent)
    {
        _parent = parent;
        OnAttachToWidget();
    }

    private void OnDetachFromWidgetInternal()
    {
        _parent = null;
        OnDetachFromWidget();
    }

    protected virtual void
        OnAttachToWidget(
        )
    {
    }


    protected virtual void OnDetachFromWidget()
    {
    }
    
    internal void ArrangeInternal(Rect bounds)
    {
        int width =Width == double.isn
            ? Math.Min(DesiredSize.Width, bounds.Width)
            : Math.Min(Width, bounds.Width);

        int height = double.IsNaN(Height)
            ? Math.Min(DesiredSize.Height, bounds.Height)
            : Math.Min(Height, bounds.Height);

        if (HorizontalAlignment == HorizontalAlignment.Stretch && double.IsNaN(Width))
        {
            width = bounds.Width;
        }

        if (VerticalAlignment == VerticalAlignment.Stretch && double.IsNaN(Height))
        {
            height = bounds.Height;
        }

        double x = HorizontalAlignment switch
        {
            HorizontalAlignment.Left or HorizontalAlignment.Stretch => bounds.X,
            HorizontalAlignment.Center => bounds.X + (bounds.Width - width) / 2d,
            HorizontalAlignment.Right => bounds.X + (bounds.Width - width),
            _ => throw new InvalidOperationException()
        };

        double y = VerticalAlignment switch
        {
            VerticalAlignment.Top or VerticalAlignment.Stretch => bounds.Y,
            VerticalAlignment.Center => bounds.Y + (bounds.Height - height) / 2d,
            VerticalAlignment.Bottom => bounds.Y + (bounds.Height - height),
            _ => throw new InvalidOperationException()
        };

        Rect finalRect = new(x, y, width, height);

        X = x;
        Y = y;
        Width = width;
        Height = height;

        Arrange(finalRect);
    }

    protected virtual void Arrange(Rect bounds)
    {
    }

    public virtual Size Measure(Size availableSize)
    {
        return Size.Zero;
    }


    /// <summary>
    /// Begins drawing the widget on the specified canvas.
    /// This method should always be called instead of <see cref="Draw(Canvas)"/>.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <exception cref="ArgumentNullException">Thrown when the canvas is null.</exception>
    public abstract void BeginDraw(Canvas canvas);

    /// <summary>
    /// Draws the widget on the specified canvas.
    /// Calling this method directly is not recommended and can lead to unexpected results.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    public abstract void Draw(Canvas canvas);

    protected void DrawBackground(Canvas canvas) =>
        canvas.DrawRectangle(BackgroundColor, (int)X, (int)Y, (int)Width, (int)Height);


    private bool ShouldRedraw<T>(T? currentValue, T? newValue)
    {
        return newValue != null
               && !EqualityComparer<T>.Default.Equals(currentValue, newValue)
               && !_isDirty
               && (currentValue is not Widget widget
                   || newValue is not Widget newWidget
                   || (!widget.IsAncestor(newWidget) && !newWidget.IsAncestor(widget)));
    }


    private void MarkDirty(bool dirty)
    {
        _isDirty = dirty;
            RenderManager.Instance.ScheduleRedraw(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(initialValue))]
    public override TValue? GetProperty<TValue>(ref TValue? field, TValue? initialValue = default)
        where TValue : default
    {
        return field ??= initialValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void SetProperty<TValue>(ref TValue? field, TValue value, string name = "") where TValue : default
        => SetProperty(ref field, value, PropertyFlag.None, name);

    protected void SetProperty<TValue>(ref TValue? field, TValue value, PropertyFlag flag = PropertyFlag.None, [CallerMemberName] string name = "",
        PropertyChangedCallback? propertyChanged = null, Binding? binding = null)
    {
        
        if (!ShouldRedraw(field, value) || )
            return;

        PropertyChanged.Invoke(name, field, value);
        propertyChanged?.Invoke(name, field, value);

        field = value;

        // Suppress redrawing if the binding does not affect visual
        if (binding is
            { Direction: BindingDirection.OneWayToTarget or BindingDirection.OneWayToSource }) return;

        MarkDirty(true);
    }

    protected PropertyBinder<TSource, TValue> Bind<TSource, TValue>(
        string propertyName
    ) where TSource : Widget
    {
        if (this is not TSource @this)
            throw new InvalidCastException(
                $"Cannot bind to {typeof(TSource).Name}. {GetType().Name} is not a {typeof(TSource).Name}.");

        TypeMetadata metadata = GetTypeMetadata<TSource>();
        if (!metadata.PropertyTable.TryGetValue(propertyName, out int offset))
            throw new FieldOffsetNotFoundException(propertyName, metadata.TypeName);

        return new PropertyBinder<TSource, TValue>(
            @this,
            propertyName,
            offset
        );
    }

    private bool IsAncestor(Widget target)
    {
        Widget? ancestor = _parent;
        while (ancestor != null)
        {
            if (this == target)
                return true;

            ancestor = ancestor._parent;
        }

        return false;
    }

    public virtual void Dispose()
    {
        if (_isDisposed)
            return;

        Bindings.RemoveAll(x =>
        {
            x.Dispose();
            return true;
        });

        OnDetachFromWidgetInternal();
        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}