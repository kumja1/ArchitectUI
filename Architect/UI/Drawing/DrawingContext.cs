using Architect.Common.Models;
using Architect.Common.Interfaces;

namespace Architect.UI.Drawing;


public class DrawingContext : IDisposable, IDrawingContext
{
    public IWidget Parent { get; set; }


    public IWidget Child { get; set; }

    public Size Size { get; set; }

    public static DrawingContext Empty => new(null, null);


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