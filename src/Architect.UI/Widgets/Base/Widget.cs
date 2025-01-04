using Architect.UI.Enums;
using Architect.UI.Models;
using Architect.UI.Extensions;
using Architect.UI.Utils;
using Architect.Common.Models;
using Architect.Common.Interfaces;
using System.Drawing;
using Size = Architect.Common.Models.Size;

namespace Architect.UI;

public class Widget : IDisposable, IWidget, IEquatable<Widget>
{
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;
    public IDrawingContext Context { get; set; }
    public bool IsVisible { get; private set; }

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
        set
        {
            if (ShouldRedraw(Content, value))
            {
                Content?.OnDetachFromWidget();
                AttachContent(value);
                BeginDraw();
            }
        }
    }

    public Color BackgroundColor
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    private bool isDirty = false;

    public Widget()
    {
        Position = Vector2.Zero;
        Context = new DrawingContext(this, null);
    }

    public virtual void OnAttachToWidget(IDrawingContext context)
    {
        Position = HorizontalAlignment switch
        {
            HorizontalAlignment.Left => Position with { X = 0 },
            HorizontalAlignment.Center => AlignmentHelper.Center(Size, context.Size),
            HorizontalAlignment.Right => AlignmentHelper.Right(Size, context.Size),
            _ => Position
        };

        Position = VerticalAlignment switch
        {
            VerticalAlignment.Top => Position with { Y = 0 },
            VerticalAlignment.Center => AlignmentHelper.Center(Size, context.Size),
            VerticalAlignment.Bottom => AlignmentHelper.Bottom(Size, context.Size),
            _ => Position
        };
    }

    public virtual void OnDetachFromWidget()
    {
        Content.Context = null;
        Context.Child = null;
    }

    public void BeginDraw()
    {
        if (Content == null) throw new ArgumentNullException(nameof(Content), "Content cannot be null when drawing the widget.");
        Erase();
        Draw();
    }

    public virtual void Draw() => Content.Draw();

    protected bool IsAncestor(IWidget widget) => GetAncestor(widget.GetType()) != null;

    protected bool IsAncestor<T>() => GetAncestor(typeof(T)) != null;

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

    protected void SetProperty<T>(ref T field, T value)
    {
        if (ShouldRedraw(field, value))
        {
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
        if (widget == null) return;
        widget.Context = Context;
        Context.Child = widget;
        widget.OnAttachToWidget(Context);
    }

    public virtual void Dispose()
    {
        Erase();
        Context.Dispose();
    }

    public void MarkDirty(bool dirty)
    {
        isDirty = dirty;
        if (dirty)
            Context.RenderManager.AddDirtyWidget(this);
    }

    public void Erase() => Context.Canvas.Clear(Size, Position);

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
}
