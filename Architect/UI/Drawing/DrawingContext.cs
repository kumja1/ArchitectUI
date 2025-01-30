using Architect.Common.Models;
using Architect.Common.Interfaces;

namespace Architect.UI.Models;


public class DrawingContext : IDisposable, IDrawingContext
{
    public IWidget Parent { get; set; }


    public IWidget Child { get; set; }

    public Size Size { get; set; }

    public static DrawingContext Empty => new(null, null);

    public Window RootWindow { get; set; }

    public DrawingContext(IWidget parent, IWidget child, Window rootWindow)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        Child = child;
        RootWindow = rootWindow;
    }

    public DrawingContext(IWidget parent, IWidget child)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        Child = child;
    }

    public void Dispose()
    {
        Child = null;
        Parent = null;
    }
}