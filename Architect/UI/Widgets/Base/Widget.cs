using Architect.Common.Models;
using Architect.UI.Enums;
using Architect.Common.Interfaces;
using Architect.UI.Models;
using Architect.Common.Utils;

namespace Architect.UI;

public abstract class Widget : IDisposable, IWidget, IEquatable<Widget>
{
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;
    public IDrawingContext Context { get; set; } = DrawingContext.Empty;
    public bool IsVisible
    {
        get; protected set;
    }
    public int ZIndex
    {
        get => field;
        set => SetProperty(ref field, value);
    }


    public Size Size
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public Vector2 Position
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public IWidget Content
    {
        get => Context.Child;
        set => SetProperty(ref field, value);
    }

    private bool isDirty;

    public Widget()
    {
        Position = Vector2.Zero;
    }


    public virtual void OnAttachToWidget(IDrawingContext context)
    {
        Position = HorizontalAlignment switch
        {
            HorizontalAlignment.Left => Position with { X = 0 },
            HorizontalAlignment.Center => AlignmentHelper.Center(context.Size, Size),
            HorizontalAlignment.Right => AlignmentHelper.Right(context.Size, Size),
            _ => Position
        };

        Position = VerticalAlignment switch
        {
            VerticalAlignment.Top => Position with { Y = 0 },
            VerticalAlignment.Center => AlignmentHelper.Center(context.Size, Size),
            VerticalAlignment.Bottom => AlignmentHelper.Bottom(context.Size, Size),
            _ => Position
        };

    }

    public virtual void OnDetachFromWidget() { }

    public void BeginDraw()
    {
        if (Content == null) throw new ArgumentNullException(nameof(Content), "Content cannot be null when drawing the widget.");
        Erase();
        Draw();
        MarkDirty(false);
    }

    public virtual void Draw() => Content.Draw();


    protected void SetProperty<T>(ref T field, T value)
    {
        if (ShouldRedraw(field!, value!))
        {
            if (value is IWidget widget)
                AttachContent(widget);
            else
                field = value;
        }
    }

    private bool ShouldRedraw(object currentValue, object newValue)
    {
        if (EqualityComparer<object>.Default.Equals(currentValue, newValue) || newValue == null || isDirty || !IsVisible) return false;
        if (currentValue is Widget widget && newValue is Widget newWidget && (widget.IsAncestor(newWidget) || newWidget.IsAncestor(widget))) return false;

        MarkDirty(true);
        return isDirty;
    }

    private void AttachContent(IWidget? widget)
    {
        Content?.Dispose();

        if (widget == null) return;
        Context = new DrawingContext(this, widget);
        widget.OnAttachToWidget(Context);
    }

    public virtual void Dispose()
    {
        Erase();
        OnDetachFromWidget();
        Context.Dispose();
    }

    public virtual void MarkDirty(bool dirty)
    {
        isDirty = dirty;
        if (dirty)
            Context.RootWindow.AddDirtyWidget(this);
    }

    public T GetRef<T>(ref T target) where T : Widget => target = (T)this;

    public void Erase() => Context.RootWindow.Erase(Position, Size);


    protected bool IsAncestor(IWidget widget) => GetAncestor(widget.GetType()) != null;

    protected bool IsAncestor<T>() where T : IWidget => GetAncestor(typeof(T)) != null;

    protected T? GetAncestor<T>() where T : class, IWidget => GetAncestor(typeof(T)) as T;

    protected IWidget? GetAncestor(Type type)
    {
        var widget = Context.Parent;
        while (widget != null)
        {
            if (widget.GetType() == type) return widget;
            widget = widget.Context.Parent;
        }
        return null;
    }

    public bool Equals(Widget? other)
    {
        if (other == null) return false;
        return HorizontalAlignment == other.HorizontalAlignment &&
               VerticalAlignment == other.VerticalAlignment &&
               Context == other.Context &&
               IsVisible == other.IsVisible &&
               ZIndex == other.ZIndex &&
               Size.Equals(other.Size) &&
               Position.Equals(other.Position) &&
               Equals(Content, other.Content);
    }

    public override bool Equals(object? obj) => Equals(obj as Widget);
    public override int GetHashCode() => HashCode.Combine(HorizontalAlignment, VerticalAlignment, Context, IsVisible, ZIndex, Size, Position, Content);

    public bool HitTest(Vector2 position) => IsVisible && PositionHelper.PositionWithin(position, Size, Position);
}
