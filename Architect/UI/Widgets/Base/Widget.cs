using Architect.Common.Models;
using Architect.Common.Interfaces;
using Architect.Common.Utilities;
using Architect.Core.Rendering;
using Size = Architect.Common.Models.Size;
using Cosmos.System.Graphics;
using Architect.UI.Widgets.Layout;
using System.Drawing;
using Architect.UI.Widgets.Bindings;
using System.Linq.Expressions;

namespace Architect.UI.Widgets.Base;

public abstract class Widget : IDisposable, IWidget
{

    private static IWidget? _previousUpdatingWidget;


    private bool _isDirty;

    public HorizontalAlignment HorizontalAlignment
    {
        get => GetProperty<HorizontalAlignment>(nameof(HorizontalAlignment));
        set => SetProperty(nameof(HorizontalAlignment), value);
    }

    public VerticalAlignment VerticalAlignment
    {
        get => GetProperty<VerticalAlignment>(nameof(VerticalAlignment));
        set => SetProperty(nameof(VerticalAlignment), value);
    }

    private protected IWidget Parent;

    private readonly List<IDisposable> _bindings = [];

    private readonly Dictionary<string, object> _properties = [];


    public Action<string, object> PropertyChanged { get; set; } = (_, _) => { };
    public bool IsVisible { get; protected set; }

    public int ZIndex
    {
        get => GetProperty<int>(nameof(ZIndex));
        set => SetProperty(nameof(ZIndex), value);
    }

    public Size Size
    {
        get => GetProperty<Size>(nameof(Size));
        set => SetProperty(nameof(Size), value);
    }

    public Vector2 Position
    {
        get => GetProperty<Vector2>(nameof(Position));
        set => SetProperty(nameof(Position), value);
    }

    public IWidget? Content
    {
        get => GetProperty<IWidget?>(nameof(Content));
        set => SetProperty(nameof(Content), value);
    }

    public Color BackgroundColor
    {
        get => GetProperty<Color>(nameof(BackgroundColor));
        set => SetProperty(nameof(BackgroundColor), value);
    }

    public Widget()
    {
        Position = Vector2.Zero;
        HorizontalAlignment = HorizontalAlignment.Left;
        VerticalAlignment = VerticalAlignment.Top;
        Size = new Size(100, 100);
    }


    public virtual void OnAttachToWidget(IWidget parent)
    {
        Arrange(parent);
        Parent = parent;
    }

    public virtual void Arrange(IWidget parent)
    {
        Position = HorizontalAlignment switch
        {
            HorizontalAlignment.Left => Position with { X = 0 },
            HorizontalAlignment.Center => AlignmentHelper.Center(parent.Size, Size),
            HorizontalAlignment.Right => AlignmentHelper.Right(parent.Size, Size),
            _ => Position
        };

        Position = VerticalAlignment switch
        {
            VerticalAlignment.Top => Position with { Y = 0 },
            VerticalAlignment.Center => AlignmentHelper.Center(parent.Size, Size),
            VerticalAlignment.Bottom => AlignmentHelper.Bottom(parent.Size, Size),
            _ => Position
        };
    }

    public virtual Size Measure(Size availableSize) => Size;

    public void BeginDraw(Canvas canvas)
    {
        if (Content == null) throw new ArgumentNullException(nameof(canvas), "Content cannot be null when drawing the widget.");

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
        var field = GetProperty<T>(name);

        if (field == null)
        {
            _properties.Add(name, value);
            PropertyChanged.Invoke(name, value);
            return;
        }

        if (ShouldRedraw(field!, value!))
        {
            if (field is IWidget currentWidget && value is IWidget newWidget)
            {
                AttachContent(ref currentWidget, newWidget);
                field = (T)(object)currentWidget;
            }
            else
            {
                if (field is HorizontalAlignment || field is VerticalAlignment)
                {
                    Arrange(Parent);
                }

                field = value;
            }


            PropertyChanged.Invoke(name, value);
            if (Parent != null && Parent == _previousUpdatingWidget) // Supress redraw if the parent is the one updating but the change is not related to the parent
                return;

            _previousUpdatingWidget = this;
            MarkDirty(true);
        }
    }


    public T? GetProperty<T>(string name)
    {
        try
        {
            return _properties.TryGetValue(name, out var currentValue) switch
            {
                true => (T)currentValue,
                false => default
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting property {name} from widget {GetType().Name}.", ex);
        }
    }


    private bool ShouldRedraw(object currentValue, object newValue)
    {
        if (newValue == null || EqualityComparer<object>.Default.Equals(currentValue, newValue) || _isDirty || !IsVisible) return false;
        if (currentValue is Widget widget && newValue is Widget newWidget && (widget.IsAncestor(newWidget) || newWidget.IsAncestor(widget))) return false;

        return true;
    }

    private void AttachContent(ref IWidget currentValue, IWidget? widget)
    {
        if (widget == null) return;
        currentValue?.Dispose();

        widget.OnAttachToWidget(this);
        currentValue = widget;
    }

    public virtual void Dispose()
    {
        RenderManager.Instance.Erase(this);

        _bindings.ForEach(x => x.Dispose());
        _bindings.Clear();
        OnDetachFromWidget();
    }

    public virtual void MarkDirty(bool dirty)
    {
        _isDirty = dirty;
        if (dirty)
            RenderManager.Instance.ScheduleRedraw(this);
    }


    protected bool IsAncestor(IWidget widget) => GetAncestor(widget.GetType()) != null;

    protected bool IsAncestor<T>() where T : IWidget => GetAncestor(typeof(T)) != null;

    public PropertyBinder<TSource, TValue> Bind<TSource, TValue>(string name, Action<TSource, TValue> setter = null) where TSource : Widget => new((TSource)this, name, _bindings, null, setter);

    public T GetRef<T>(ref T target) where T : Widget => target = (T)this;


    protected T? GetAncestor<T>() where T : class, IWidget => GetAncestor(typeof(T)) as T;

    protected IWidget? GetAncestor(Type type)
    {
        var widget = Parent;
        while (widget != null)
        {
            if (widget.GetType() == type) return widget;
            widget = ((Widget)widget).Parent;
        }
        return null;
    }


    public virtual void OnDetachFromWidget() { }


    public override int GetHashCode() => HashCode.Combine(HorizontalAlignment, VerticalAlignment, Parent, IsVisible, ZIndex, Size, Position, Content);

    public bool HitTest(Vector2 position) => IsVisible && Position.X >= position.X && Position.X <= position.X + Size.Width && Position.Y >= position.Y && Position.Y <= position.Y + Size.Height;
}
