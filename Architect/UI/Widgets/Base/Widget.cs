using Architect.Common.Models;
using Architect.Common.Interfaces;
using Architect.Common.Utils;
using Architect.Core.Rendering;
using Size = Architect.Common.Models.Size;
using Cosmos.System.Graphics;
using Architect.UI.Layout;

namespace Architect.UI.Base;

public abstract class Widget : IDisposable, IWidget
{
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;

    private bool IsDirty { get; set; }

    private protected IWidget Parent
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public bool IsVisible { get; protected set; }
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

    public Widget()
    {
        Position = Vector2.Zero;
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

    public virtual void OnDetachFromWidget() { }

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
            if (field is IWidget currentWidget && value is IWidget newWidget)
            {
                AttachContent(ref currentWidget, newWidget);
                field = (T)(object)currentWidget;
            }
            else
                field = value;

            MarkDirty(true);
        }
    }

    private bool ShouldRedraw(object currentValue, object newValue)
    {
        if (newValue == null || EqualityComparer<object>.Default.Equals(currentValue, newValue) || IsDirty || !IsVisible) return false;
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
        OnDetachFromWidget();
    }

    public virtual void MarkDirty(bool dirty)
    {
        IsDirty = dirty;
        if (dirty)
            RenderManager.Instance.ScheduleRedraw(this);
    }


    protected bool IsAncestor(IWidget widget) => GetAncestor(widget.GetType()) != null;

    protected bool IsAncestor<T>() where T : IWidget => GetAncestor(typeof(T)) != null;

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

    public override int GetHashCode() => HashCode.Combine(HorizontalAlignment, VerticalAlignment, Parent, IsVisible, ZIndex, Size, Position, Content);

    public bool HitTest(Vector2 position) => IsVisible && Position.X >= position.X && Position.X <= position.X + Size.Width && Position.Y >= position.Y && Position.Y <= position.Y + Size.Height;
}
