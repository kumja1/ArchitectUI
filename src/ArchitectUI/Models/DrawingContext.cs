
using Architect.Widgets;
using Cosmos.System.Graphics;

class DrawingContext : IDisposable
{
    public Widget Parent { get; set; }
    public Widget Child { get; set; }
    public Canvas Canvas { get; set; }

    public DrawingContext(Widget parent, Widget child)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        Child = child;
        Canvas = parent.Context?.Canvas ?? FullScreenCanvas.GetCurrentFullScreenCanvas();
        if (child != null) child.Context = this;
    }

    public void Dispose()
    {
        Child = null;
        Parent = null;
    }
}
