using System.Drawing;
using Architect.UI.Enums;
using Architect.UI.Models;
using Architect.UI.Extensions;
using Architect.UI.Utils;
using Size = Architect.UI.Models.Size;

namespace Architect.UI;

public abstract class Widget : IDisposable
{

    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;

    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;

    public DrawingContext Context { get; set; }
    public bool IsVisible { get; private set; }

    private protected bool isDirty = false;

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

    public Widget Content
    {
        get => Context.Child;
        set
        {
            if (ShouldRedraw(Content, value))
            {
                AttachContent(value);
                Draw();
            }
        }
    }


    public Widget()
    {
        Context = new DrawingContext(this, null);
        Position = Vector2.Zero;
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

    public void BeginDraw()
    {
        if (!isDirty || !IsVisible) return;
        Context.Canvas.Clear(Size, Position);
        Draw();
        isDirty = false;

    }

    public virtual void Draw() => Content?.Draw();


    protected private bool IsAncestor(Widget widget)
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

    protected private bool ShouldRedraw(object currentValue, object newValue)
    {
        if (currentValue == newValue || newValue == null || isDirty) return false;
        if (currentValue is Widget widget && newValue is Widget newWidget)
        {
            if (widget.IsAncestor(newWidget)) return false;
        }
        isDirty = true;
        return isDirty;
    }

    private void AttachContent(Widget? widget)
    {
        if (widget == null) return;
        widget.Context = Context;
        Context.Child = widget;
        widget.OnAttachToWidget(Context);
    }

    public void Dispose() => Context.Dispose();





}
