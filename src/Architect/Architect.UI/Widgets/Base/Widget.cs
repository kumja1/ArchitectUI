using System.Drawing;
using Architect.Common.Interfaces;
using Architect.Common.Models;
using Architect.Common.Utilities;
using Architect.Common.Utilities.Extensions;
using Architect.Core.Rendering;
using Architect.UI.Data.Core;
using Architect.UI.Data.Interfaces;
using Architect.UI.Widgets.Layout;
using Cosmos.System.Graphics;
using Size = Architect.Common.Models.Size;

namespace Architect.UI.Widgets.Base;

public class Widget : IDisposable, IWidget, IBindable
{
    // private static IWidget? _previousUpdatingWidget;
    private bool _isDirty;
    private bool _isDisposed;
    private Vector2 _lastPosition;
    protected Size? _measuredSize;
    protected Size? _naturalSize;
    private readonly List<IDisposable> _bindings = new();
    private readonly Dictionary<string, object> _properties = new();

    public event Action<string, object> PropertyChanged = delegate { };

    protected IWidget? InternalContent;
    protected IWidget? Parent;

    public Size MeasuredSize => _measuredSize.Value;
    public Vector2 LastPosition => _lastPosition;

    public HorizontalAlignment HorizontalAlignment
    {
        get => GetProperty(nameof(HorizontalAlignment), defaultValue: HorizontalAlignment.Left);
        set => SetProperty(nameof(HorizontalAlignment), value);
    }

    public VerticalAlignment VerticalAlignment
    {
        get => GetProperty(nameof(VerticalAlignment), defaultValue: VerticalAlignment.Top);
        set => SetProperty(nameof(VerticalAlignment), value);
    }

    public bool IsVisible
    {
        get => GetProperty(nameof(IsVisible), true);
        set => SetProperty(nameof(IsVisible), value);
    }

    public int ZIndex
    {
        get => GetProperty<int>(nameof(ZIndex));
        set => SetProperty(nameof(ZIndex), value);
    }

    public double X
    {
        get => GetProperty(nameof(X), defaultValue: 0);
        set => SetProperty(nameof(X), value);
    }

    public double Y
    {
        get => GetProperty(nameof(Y), defaultValue: 0);
        set => SetProperty(nameof(Y), value);
    }

    public double SizeX
    {
        get => GetProperty(nameof(SizeX), defaultValue: 0);
        set => SetProperty(nameof(SizeX), value);
    }

    public double SizeY
    {
        get => GetProperty(nameof(SizeY), defaultValue: 0);
        set => SetProperty(nameof(SizeY), value);
    }

    public IWidget? Content
    {
        get => GetProperty<IWidget>(nameof(Content));
        set => SetProperty(nameof(Content), value);
    }

    public Color BackgroundColor
    {
        get => GetProperty(nameof(BackgroundColor), defaultValue: Color.White);
        set => SetProperty(nameof(BackgroundColor), value);
    }

    public EdgeInsets Padding
    {
        get => GetProperty(nameof(Padding), defaultValue: EdgeInsets.Zero);
        set => SetProperty(nameof(Padding), value);
    }

    public EdgeInsets Margin
    {
        get => GetProperty(nameof(Margin), defaultValue: EdgeInsets.Zero);
        set => SetProperty(nameof(Margin), value);
    }

    public virtual void OnAttachToWidget(IWidget parent) => Parent = parent;

    public virtual void OnDetachFromWidget() => Parent = null;

    protected virtual void OnPropertyChanged(string name, object previousValue, object value)
    {
        switch (name)
        {
            case nameof(Content):
                AttachContent(ref previousValue, (IWidget)value);
                break;
            case nameof(HorizontalAlignment):
            case nameof(VerticalAlignment):
            case nameof(Padding):
                Parent?.MarkDirty(true);
                break;
        }

        PropertyChanged?.Invoke(name, value);
    }

    public virtual void Arrange(Rect finalRect)
    {
        var desiredWidth =
            HorizontalAlignment == HorizontalAlignment.Stretch
                ? finalRect.Size.Width
                : Math.Min(SizeX, finalRect.Size.Width);

        var desiredHeight =
            VerticalAlignment == VerticalAlignment.Stretch
                ? finalRect.Size.Height
                : Math.Min(SizeY, finalRect.Size.Height);

        var offset = GetOffset(finalRect.Size, desiredWidth, desiredHeight);

        // Update the scalar properties directly
        X = finalRect.Position.X + offset.X;
        Y = finalRect.Position.Y + offset.Y;
        SizeX = desiredWidth;
        SizeY = desiredHeight;

        ArrangeContent();
    }

    protected virtual void ArrangeContent()
    {
        var contentArea = new Rect(
            X + Padding.Left,
            Y + Padding.Top,
            SizeX - Padding.Size.Width,
            SizeY - Padding.Size.Height
        );
        InternalContent?.Arrange(contentArea);
    }

    public virtual Size Measure(Size availableSize = default)
    {
        if (!IsVisible || InternalContent == null)
            return (_measuredSize = Size.Zero).Value;

        var contentAvailable = availableSize - Padding.Size;
        var contentSize = InternalContent?.Measure(contentAvailable) ?? Size.Zero;
        _measuredSize = contentSize + Padding.Size;
        return _measuredSize.Value;
    }

    public virtual Size GetNaturalSize() =>
        Padding.Size + InternalContent?.GetNaturalSize() ?? Size.Zero;

    private Vector2 GetOffset(Size finalSize, double desiredWidth, double desiredHeight)
    {
        var tempSize = new Size(desiredWidth, desiredHeight);
        return new Vector2(
            HorizontalAlignment switch
            {
                HorizontalAlignment.Left => 0,
                HorizontalAlignment.Center => AlignmentHelper.AlignCenterX(finalSize, tempSize),
                HorizontalAlignment.Right => AlignmentHelper.AlignRight(finalSize, tempSize),
                _ => throw new InvalidOperationException("Invalid HorizontalAlignment value."),
            },
            VerticalAlignment switch
            {
                VerticalAlignment.Top => 0,
                VerticalAlignment.Center => AlignmentHelper.AlignCenterY(finalSize, tempSize),
                VerticalAlignment.Bottom => AlignmentHelper.AlignBottom(finalSize, tempSize),
                _ => throw new InvalidOperationException("Invalid VerticalAlignment value."),
            }
        );
    }

    /// <summary>
    /// Begins drawing the widget on the specified canvas.
    /// Always call this method instead of directly calling <see cref="Draw(Canvas)"/>.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <exception cref="ArgumentNullException">Thrown when the canvas is null.</exception>
    public virtual void BeginDraw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));

        if (InternalContent == null || InternalContent.Content == null)
            throw new ArgumentNullException(nameof(Content), "Content cannot be null.");

        Draw(canvas);
        MarkDirty(false);
    }

    /// <summary>
    /// Draws the widget on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    public virtual void Draw(Canvas canvas)
    {
        DrawBackground(canvas);
        InternalContent!.Draw(canvas);
    }

    protected void DrawBackground(Canvas canvas) =>
        canvas.DrawRectangle(BackgroundColor, (int)X, (int)Y, (int)SizeX, (int)SizeY);

    /// <summary>
    /// Sets the value of a property and marks the widget as dirty if the value has changed.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="name">The name of the property.</param>
    /// <param name="value">The new value of the property.</param>
    /// <param name="associatedBinding">The binding associated with the property, if any.</param>
    public void SetProperty<T>(string name, T value, IBinding? associatedBinding = null)
    {
        var currentValue = GetProperty<T>(name);

        if (!ShouldRedraw(currentValue, value))
            return;

        _properties[name] = value;
        OnPropertyChanged(name, currentValue, value);

        if (associatedBinding != null)
        {
            var direction = associatedBinding.Direction;
            if (
                direction == BindingDirection.OneWayToTarget
                || direction == BindingDirection.OneWayToSource
            )
                return;
        }

        MarkDirty(true);
    }

    public T? GetProperty<T>(string name, T? defaultValue = default)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        try
        {
            return _properties.GetOrAddValue(name, defaultValue);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting property {name} from widget {this}.", ex);
        }
    }

    private bool ShouldRedraw(object? currentValue, object? newValue)
    {
        if (
            newValue == null
            || EqualityComparer<object>.Default.Equals(currentValue, newValue)
            || _isDirty
            || (
                currentValue is Widget widget
                && newValue is Widget newWidget
                && (widget.IsAncestor(newWidget) || newWidget.IsAncestor(widget))
            )
        )
            return false;

        return true;
    }

    protected virtual void AttachContent(ref object currentValue, object? widget)
    {
        if (currentValue is Widget currentWidget && widget is Widget newWidget)
        {
            _naturalSize = null;
            currentWidget.Dispose();
            newWidget.OnAttachToWidget(this);

            if (InternalContent == null)
            {
                InternalContent = newWidget;
                return;
            }

            InternalContent.Content?.Dispose();
            InternalContent.Content = newWidget;
        }
    }

    public virtual void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        RenderManager.Instance.Erase(this);
        _bindings.RemoveAll(x =>
        {
            x.Dispose();
            return true;
        });

        OnDetachFromWidget();
    }

    public virtual void MarkDirty(bool dirty)
    {
        _isDirty = dirty;
        if (dirty)
            RenderManager.Instance.ScheduleRedraw(this);
    }

    public PropertyBinder<TSource, TValue> Bind<TSource, TValue>(
        string name,
        Action<TSource, TValue, IBinding?>? setter = null
    )
        where TSource : Widget =>
        new(
            this as TSource
                ?? throw new InvalidCastException($"Unable to cast {this} to {typeof(TSource)}"),
            name,
            _bindings,
            null,
            setter
        );

    /// <summary>
    /// Gets a reference to the current widget.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <returns>A reference to the current widget</returns>
    public T GetReference<T>(out T target)
        where T : Widget => target = (T)this;

    protected bool IsAncestor(IWidget widget) => GetAncestor(widget.GetType()) != null;

    protected bool IsAncestor<T>()
        where T : IWidget => GetAncestor(typeof(T)) != null;

    protected T? GetAncestor<T>()
        where T : class, IWidget => GetAncestor(typeof(T)) as T;

    protected IWidget? GetAncestor(Type type)
    {
        var widget = Parent;
        while (widget != null)
        {
            if (widget.GetType() == type)
                return widget;
            widget = (widget as Widget)?.Parent;
        }
        return null;
    }

    public bool HitTest(Vector2 position) =>
        position.Within(new Vector2(X, Y), new Vector2(X + SizeX, Y + SizeY));
}
