using Architect.UI.Enums;
using Architect.UI.Models;
using Architect.UI.Extensions;
using Architect.UI.Utils;
using Architect.Common.Models;
using Architect.Common.Interfaces;
using System.Drawing;
using Size = Architect.Common.Models.Size;

namespace Architect.UI;

public class Widget : IDisposable, IWidget
{
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;
    public IDrawingContext Context { get; set; }
    public bool IsVisible { get; private set; }

    public int ZIndex { get => field; set => SetProperty(ref field, value); }

    private bool isDirty = false;
    
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

    public Color BackgroundColor
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
                AttachContent(value);
                BeginDraw();
            }
        }
    }

    public Widget()
    {
        Position = Vector2.Zero;
        Context = new DrawingContext(this, null);
        BackgroundColor = Color.Transparent;
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

    public virtual void OnDetachFromWidget() => Content?.Dispose();

    public void BeginDraw()
    {
        if (Content == null) throw new ArgumentNullException(nameof(Content), "Content cannot be null when drawing the widget.");
        Context.Canvas.Clear(Size, Position);
        Draw();
    }

    public virtual void Draw() => Content.Draw();

    protected private bool IsAncestor(IWidget widget)
    {
        while (widget != null)
        {
            if (widget == this) return true;
            widget = widget.Context.Parent;
        }
        return false;
    }

    protected private void SetProperty<T>(ref T field, T value)
    {
        if (ShouldRedraw(field, value))
        {
            field = value;
        }
    }

    private protected bool ShouldRedraw(object currentValue, object newValue)
    {
        if (newValue == null || currentValue == newValue || isDirty || !IsVisible) return false;
        if (currentValue is Widget widget && newValue is Widget newWidget && (widget.IsAncestor(newWidget) || newWidget.IsAncestor(widget))) return false;

        Context.RenderManager.AddDirtyWidget(this);
        return isDirty;
    }

    private void AttachContent(IWidget? widget)
    {
        if (widget == null) return;
        widget.Context = Context;
        Context.Child = widget;

        widget.OnAttachToWidget(Context);
    }

    public void Dispose() => Context.Dispose();

    public void MarkDirty(bool dirty) => isDirty = dirty;
}