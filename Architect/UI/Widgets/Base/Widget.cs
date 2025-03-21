using System.Drawing;
using Architect.Common.Interfaces;
using Architect.Common.Models;
using Architect.Common.Utilities;
using Architect.Common.Utilities.Extensions;
using Architect.Core.Rendering;
using Architect.UI.Widgets.Bindings;
using Architect.UI.Widgets.Layout;
using Cosmos.System.Graphics;
using Size = Architect.Common.Models.Size;

namespace Architect.UI.Widgets.Base;

public class Widget : IDisposable, IWidget
{
    private static IWidget? _previousUpdatingWidget;

    private bool _isDirty;

    private readonly List<IDisposable> _bindings = [];

    private readonly Dictionary<string, object> _properties = [];

    public event Action<string, object> PropertyChanged;

    private protected IWidget Parent
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

    public bool IsVisible { get; protected set; }

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
        get => GetProperty<IWidget?>(nameof(Content), defaultValue: null);
        set => SetProperty(nameof(Content), value);
    }

    public Color BackgroundColor
    {
        get => GetProperty(nameof(BackgroundColor), defaultValue: Color.White);
        set => SetProperty(nameof(BackgroundColor), value);
    }

    protected Widget() { }

    public virtual void OnAttachToWidget(IWidget parent) => Parent = parent;

    public virtual void OnDetachFromWidget() { }

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
            Arrange();
        }

        PropertyChanged?.Invoke(name, value);
    }

    public virtual void Arrange()
    {
        var x = HorizontalAlignment switch
        {
            HorizontalAlignment.Left => 0,
            HorizontalAlignment.Right => AlignmentHelper.Right(Parent.Size, Size).X,
            HorizontalAlignment.Center => AlignmentHelper.Center(Parent.Size, Size).X,
            HorizontalAlignment.Stretch => Size.Width,
            _ => Position.X,
        };

        var y = VerticalAlignment switch
        {
            VerticalAlignment.Top => 0,
            VerticalAlignment.Bottom => AlignmentHelper.Bottom(Parent.Size, Size).Y,
            VerticalAlignment.Center => AlignmentHelper.Center(Parent.Size, Size).Y,
            VerticalAlignment.Stretch => Size.Height,
            _ => Position.Y,
        };

        Position = new Vector2(x, y);
    }

    public virtual Size Measure(Size availableSize) => Size;

    public void BeginDraw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));

        Draw(canvas);
        MarkDirty(false);
    }

    public virtual void Draw(Canvas canvas)
    {
        canvas.DrawRectangle(BackgroundColor, Position.X, Position.Y, Size.Width, Size.Height);
        Content!.Draw(canvas);
    }

    public void SetProperty<T>(string name, T value)
    {
        var currentValue = GetProperty<T>(name);
        if (ShouldRedraw(currentValue, value))
        {
            _properties[name] = value;
            OnPropertyChanged(name, currentValue, value);
            if (Parent != null && Parent == _previousUpdatingWidget) // Supress redraw if the parent is the one updating but the change is not related to the parent
                return;

            _previousUpdatingWidget = this;
            MarkDirty(true);
        }
    }

    public T? GetProperty<T>(string name, T? defaultValue = default)
    {
        try
        {
            return _properties.GetOrAddValue(name, defaultValue);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting property {name} from widget {GetType().Name}.", ex);
        }
    }

    private bool ShouldRedraw(object? currentValue, object newValue)
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
        Action<TSource, TValue>? setter = null
    )
        where TSource : Widget => new((TSource)this, name, _bindings, null, setter);

    public T GetRef<T>(ref T target)
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
