
using Cosmos.System.Graphics;

namespace Architect.UI.Models;

/// <summary>
/// Represents the drawing context between a parent and child widget.
/// </summary>
public class DrawingContext : IDisposable
{
    public Widget Parent { get; set; }
    public Widget Child { get; set; }
    public Canvas Canvas { get; set; }
    public Size Size { get; set; }

    public DrawingContext(Widget parent, Widget child)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        Child = child;
        Canvas = parent.Context.Canvas;
        if (child != null) child.Context = this;
    }

    public DrawingContext(Widget parent, Widget child, Canvas canvas)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        Child = child;
        Canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        if (child != null) child.Context = this;
    }

    public void Dispose()
    {
        Child = null;
        Parent = null;
        Canvas = null;
    }
}
