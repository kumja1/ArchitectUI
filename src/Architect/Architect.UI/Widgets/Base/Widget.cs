using System.Drawing;
using Architect.Common.Interfaces;
using Architect.Common.Models;
using Architect.Common.Utilities;
using Architect.Common.Utilities.Extensions;
using Architect.Core.Rendering;
using Architect.UI.Widgets.Binding.Core;
using Architect.UI.Widgets.Binding.Interfaces;
using Architect.UI.Widgets.Layout;
using Cosmos.System.Graphics;
using Size = Architect.Common.Models.Size;

namespace Architect.UI.Widgets.Base;

public class Widget : IDisposable, IWidget, IBindable
{
    // private static IWidget? _previousUpdatingWidget;

    private bool _isDirty;

    private protected Size? _naturalSize;

    private readonly List<IDisposable> _bindings = [];

    private readonly Dictionary<string, object> _properties = [];

    public event Action<string, object> PropertyChanged = delegate { };

    protected IWidget InternalContent;

    protected IWidget? Parent
    {
        get => GetProperty<IWidget>(nameof(Parent));
        private set => SetProperty(nameof(Parent), value);
    }

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

    public Widget()
    {
        InternalContent ??= Content;
    }

    public virtual void OnAttachToWidget(IWidget parent) => Parent = parent;

    public virtual void OnDetachFromWidget() => Parent = null;

    public virtual Size Measure(Size availableSize = default)
    {
        _naturalSize ??= GetNaturalSize();
        return Size.Clamp(_naturalSize.Value, Size, availableSize);
    }

    public virtual Size GetNaturalSize() => Size + InternalContent?.GetNaturalSize() ?? Size.Zero;

    protected virtual void OnPropertyChanged(string name, object currentValue, object value)
    {
        if (name == nameof(Content) && value is IWidget newWidget)
        {
            var currentWidget = (IWidget)currentValue;
            AttachContent(ref currentWidget, newWidget);
        }
        else if (
            name == nameof(Size)
            || name == nameof(HorizontalAlignment)
            || name == nameof(VerticalAlignment)
        )
        {
            var finalRect = new Rect(Position, Measure(Parent?.Size ?? Size.Infinite));
            Arrange(finalRect);
        }

        PropertyChanged?.Invoke(name, value);
    }

    public virtual void Arrange(Rect finalRect)
    {
        var x = HorizontalAlignment switch
        {
            HorizontalAlignment.Left => 0,
            HorizontalAlignment.Right => AlignmentHelper.Right(finalRect.Size, Size).X,
            HorizontalAlignment.Center => AlignmentHelper.Center(finalRect.Size, Size).X,
            HorizontalAlignment.Stretch => Size.Width,
            _ => Position.X,
        };

        var y = VerticalAlignment switch
        {
            VerticalAlignment.Top => 0,
            VerticalAlignment.Bottom => AlignmentHelper.Bottom(finalRect.Size, Size).Y,
            VerticalAlignment.Center => AlignmentHelper.Center(finalRect.Size, Size).Y,
            VerticalAlignment.Stretch => Size.Height,
            _ => Position.Y,
        };

        Size = finalRect.Size;
        Position = new Vector2(x, y);
    }

    public virtual void BeginDraw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));

        if (InternalContent == null)
            throw new ArgumentNullException(nameof(Content), "Content cannot be null.");

        Draw(canvas);
        MarkDirty(false);
    }

    public virtual void Draw(Canvas canvas)
    {
        DrawBackground(canvas);
        InternalContent!.Draw(canvas);
    }

    private protected void DrawBackground(Canvas canvas) =>
        canvas.DrawRectangle(BackgroundColor, Position.X, Position.Y, Size.Width, Size.Height);

    public void SetProperty<T>(string name, T value, IBinding? associatedBinding = null)
    {
        var currentValue = GetProperty<T>(name);

        if (ShouldRedraw(currentValue, value))
        {
            _properties[name] = value;
            OnPropertyChanged(name, currentValue, value);

            if (associatedBinding != null)
            {
                var (_, _, direction) = associatedBinding;
                if (
                    direction == BindingDirection.OneWayToTarget
                    || direction == BindingDirection.OneWayToSource
                )
                    return; // If the binding is one way, we don't need to update the property. This is because the property does not directly impact the widget itself, so we defer the update and redrawing to the target widget.
            }

            MarkDirty(true);
        }
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

    private void AttachContent(ref IWidget currentValue, IWidget? widget)
    {
        if (widget == null)
            return;
        currentValue?.Dispose();
        widget.OnAttachToWidget(this);
    }

    public virtual void Dispose()
    {
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
