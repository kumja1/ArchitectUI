using System.Drawing;
using Architect.Enums;
using Architect.Models;
using Architect.Utils;

namespace Architect.Widgets;

abstract class Widget : IDisposable
{

    public HorizontalAlignment HorizontalAlignment
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            Draw();
        }
    }


    public VerticalAlignment VerticalAlignment
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            Draw();
        }
    }

    public Size Size
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            Draw();
        }
    }
    public Vector2 Position
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            Draw();
        }
    }
    public Color BackgroundColor
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            Draw();
        }
    }

    public Widget Content
    {
        get => Context.Child;
        set
        {
            if (Context.Child == value) return;
            Context.Child = value;
        }
    }

    public DrawingContext Context { get; set; }

    public Widget()
    {
        Position = Vector2.Zero;
        Size = new Size(1, 1);
        BackgroundColor = Color.White;
        Context = new DrawingContext(this, null);
    }

    public abstract void Draw();

    public void Dispose() => Context.Dispose();
}
