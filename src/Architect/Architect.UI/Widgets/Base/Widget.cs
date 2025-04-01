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

    protected Size? _measuredSize;

    protected Size? _naturalSize;

    private readonly List<IDisposable> _bindings = [];

    private readonly Dictionary<string, object> _properties = [];

    public event Action<string, object> PropertyChanged = delegate { };

    protected IWidget? InternalContent;

    protected IWidget? Parent;

    public Size MeasuredSize => _measuredSize.Value;

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

    public Size Size
    {
        get => GetProperty(nameof(Size), defaultValue: Size.Zero);
        set => SetProperty(nameof(Size), value);
    }

    public Vector2 Position
    {
        get => GetProperty(nameof(Position), defaultValue: Vector2.Zero);
        set => SetProperty(nameof(Position), value);
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
        // Remove _layoutArea calculation from here - it belongs in Arrange
        switch (name)
        {
            case nameof(Content):
                AttachContent(ref previousValue, (IWidget)value);
                break;
            case nameof(Size):
            case nameof(HorizontalAlignment):
            case nameof(VerticalAlignment):
            case nameof(Padding):
                // Don't self-arrange - request parent layout
                Parent?.MarkDirty(true);
                break;
            case nameof(Position):
                // Position changes should trigger parent re-layout
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
                : Math.Min(Size.Width, finalRect.Size.Width);

        var desiredHeight =
            VerticalAlignment == VerticalAlignment.Stretch
                ? finalRect.Size.Height
                : Math.Min(Size.Height, finalRect.Size.Height);

        var offset = GetOffset(finalRect.Size, desiredWidth, desiredHeight);

        Size = new Size(desiredWidth, desiredHeight);
        Position = finalRect.Position + offset;
        ArrangeContent();
    }

    protected virtual void ArrangeContent()
    {
        var contentArea = new Rect(
            Position.X + Padding.Left,
            Position.Y + Padding.Top,
            Size.Width - Padding.Size.Width,
            Size.Height - Padding.Size.Height
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

    public virtual Size GetNaturalSize() => InternalContent?.GetNaturalSize() ?? Size.Zero;

    private Vector2 GetOffset(Size finalSize, float desiredWidth, float desiredHeight)
    {
        var size = new Size(desiredWidth, desiredHeight);
        return HorizontalAlignment switch
            {
                HorizontalAlignment.Left => new Vector2(0, 0),
                HorizontalAlignment.Center => AlignmentHelper.CenterX(finalSize, size),
                HorizontalAlignment.Right => AlignmentHelper.Right(finalSize, size),
                _ => throw new InvalidOperationException("Invalid HorizontalAlignment value."),
            }
            + VerticalAlignment switch
            {
                VerticalAlignment.Top => new Vector2(0, 0),
                VerticalAlignment.Center => AlignmentHelper.CenterY(finalSize, size),
                VerticalAlignment.Bottom => AlignmentHelper.Bottom(finalSize, size),
                _ => throw new InvalidOperationException("Invalid VerticalAlignment value."),
            };
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
        canvas.DrawRectangle(
            BackgroundColor,
            (int)Position.X,
            (int)Position.Y,
            (int)Size.Width,
            (int)Size.Height
        );

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
                return; // If the binding is one way, we don't need to update the property. This is because the property does not directly impact the widget itself, so we defer the update and redrawing to the target widget.
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

            currentWidget?.Dispose();
            newWidget.OnAttachToWidget(this);

            // If internal content is null, we set the new widget(content) as the internal content
            if (InternalContent == null)
            {
                InternalContent = newWidget;
                return;
            }

            // If internal content has an impl, we replace the impl content with the new widget
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
        position.X >= Position.X
        && position.X <= Position.X + Size.Width
        && position.Y >= Position.Y
        && position.Y <= Position.Y + Size.Height;

    public override int GetHashCode() =>
        HashCode.Combine(
            HorizontalAlignment,
            VerticalAlignment,
            Parent,
            IsVisible,
            ZIndex,
            Size,
            Position,
            Content
        );
}
