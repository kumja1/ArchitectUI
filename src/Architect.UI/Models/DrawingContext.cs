using Architect.Common.Interfaces;
using Architect.Common.Models;
using Cosmos.System.Graphics;

namespace Architect.UI.Models;

/// <summary>
/// Represents the drawing context between a parent and child widget.
/// </summary>
public class DrawingContext : IDisposable, IDrawingContext
{
    public IWidget Parent { get; set; }
    public IWidget Child { get; set; }
    public Canvas Canvas { get; set; }
    public IRenderManager RenderManager => Parent.Context.RenderManager;
    public Size Size { get; set; }

    public DrawingContext(IWidget parent, IWidget child)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        Child = child;
        Canvas = parent.Context.Canvas;
        if (child != null) child.Context = this;
    }

    public DrawingContext(IWidget parent, IWidget child, Canvas canvas)
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