using Architect.Common.Models;
using Architect.Common.Interfaces;
using Architect.Common.Utils;
using Architect.UI.Drawing;
using Architect.Core.Rendering;
using Size = Architect.Common.Models.Size;
using Cosmos.System.Graphics;
using Architect.UI.Layout;

namespace Architect.UI.Base;

public abstract class Widget : IDisposable, IWidget
{
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;
    public IDrawingContext Context { get; set; } = DrawingContext.Empty;

    public


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
        get => field;
        set => SetProperty(ref field, value);
    }


    private bool isDirty;

    public Widget()
    {
        Position = Vector2.Zero;
    }


    public virtual void OnAttachToWidget(IDrawingContext context)
    {
        Context = context;
    }

    public virtual void OnDetachFromWidget() => Context = DrawingContext.Empty;

    public void BeginDraw(Canvas canvas)
    {
        if (Content == null) throw new ArgumentNullException(nameof(canvas), "Content cannot be null when drawing the widget.");

        Draw(canvas);
        MarkDirty(false);
    }

    public virtual void Draw(Canvas canvas) => Content.Draw(canvas);

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
        widget.OnAttachToWidget(new DrawingContext(this, widget));
    }

    public virtual void Dispose()
    {
        RenderManager.Instance.Erase(this);
        OnDetachFromWidget();
    }

    public virtual void MarkDirty(bool dirty)
    {
        isDirty = dirty;
        if (dirty)
            RenderManager.Instance.ScheduleRedraw(this);
    }

    public T GetRef<T>(ref T target) where T : Widget => target = (T)this;


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

    public override int GetHashCode() => HashCode.Combine(HorizontalAlignment, VerticalAlignment, Context, IsVisible, ZIndex, Size, Position, Content);

    public bool HitTest(Vector2 position) => IsVisible && PositionHelper.PositionWithin(position, Size, Position);
}
