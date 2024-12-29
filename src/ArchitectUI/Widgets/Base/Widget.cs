using System.Drawing;
using Architect.Enums;
using Architect.Models;
using Architect.Utils;
using ArchitectUI.Utils;

namespace Architect.Widgets;

abstract class Widget : IDisposable
{

    public HorizontalAlignment HorizontalAlignment { get; set; }

    public VerticalAlignment VerticalAlignment { get; set; }

    public DrawingContext Context { get; set; }
    public bool IsVisible { get; private set; }

    private bool isDirty = false;

    public Size Size
    {
        get => field;
        set
        {
            if (ShouldRedraw(Size, value))
            {
                field = value;
                Draw();
            }
        }
    }
    public Vector2 Position
    {
        get => field;
        set
        {
            if (ShouldRedraw(Position, value))
            {
                field = value;
                Draw();
            }
        }
    }
    public Color BackgroundColor
    {
        get => field;
        set
        {
            if (ShouldRedraw(BackgroundColor, value))
            {
                field = value;
                Draw();
            }
        }
    }

    public Widget Content
    {
        get => Context.Child;
        set
        {
            if (ShouldRedraw(Context.Child, value))
            {
                AttachContent(value);
                Draw();
            }
        }
    }


    private protected bool ShouldRedraw(object currentValue, object newValue)
    {
        if (currentValue == newValue || newValue == null) return false;
        if (currentValue is Widget widget && newValue is Widget newWidget)
        {
            if (widget.IsAncestor(newWidget)) return false;
        }
        isDirty = true;
        return isDirty;
    }


    private void AttachContent(Widget widget)
    {
        if (widget == null) return;
        widget.Context = Context;
        Context.Child = widget;
        widget.OnAttachToWidget(Context);
    }

    public Widget()
    {
        Context = new DrawingContext(this, null);
        Position = AlignmentHelper.Center(Context.Parent.Size, Size);
        Size = Size.Clamp(Size.Zero, Context.Parent.Size, Size.Infinite);
        BackgroundColor = Color.Transparent;
    }

    public virtual void OnAttachToWidget(DrawingContext context)
    {
        switch (HorizontalAlignment)
        {
            case HorizontalAlignment.Left:
                Position = Position with { X = 0 };
                break;
            case HorizontalAlignment.Center:
                Position = AlignmentHelper.Center(Size, context.Size);
                break;
            case HorizontalAlignment.Right:
                Position = AlignmentHelper.Right(Size, context.Size);
                break;
        }

        switch (VerticalAlignment)
        {
            case VerticalAlignment.Top:
                Position = Position with { Y = 0 };
                break;
            case VerticalAlignment.Center:
                Position = AlignmentHelper.Center(Size, context.Size);
                break;
            case VerticalAlignment.Bottom:
                Position = AlignmentHelper.Bottom(Size, context.Size);
                break;
        }
    }

    public virtual void OnDetachFromWidget() => Content?.Dispose();

    public virtual void Draw()
    {
        if (!IsVisible) return;

        if (isDirty)
        {
            if (Content != null)
            {
                Content.Draw();
            }
            else
            {
                Context.Canvas.DrawRectangle(BackgroundColor, Position.X, Position.Y, Size.Width, Size.Height);
            }
            isDirty = false;
        }
    }

    public void Dispose() => Context.Dispose();

    protected private bool IsAncestor(Widget widget)
    {
        while (widget != null)
        {
            if (widget == this) return true;
            widget = widget.Context.Parent;
        }
        return false;
    }
}
